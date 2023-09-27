using System;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.XR;
using GorillaLocomotion;
using System.IO;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using Green_Screen_Mod;
using GorillaExtensions;

namespace Green_Screen_Mod_Gtag
{
    [BepInPlugin(PluginInfo.modGUID, PluginInfo.modName, PluginInfo.modVersion)]
    public class MainPatch : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony(PluginInfo.modGUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            this.gameObject.AddComponent<InputController>();
            this.gameObject.AddComponent<PaintPallet>();
        }
        public static int FramePressCoolDown = 0;
        static int BtnCooldown;

        public static GameObject GreenScreenOBJ = null;
        public static GameObject Dropper = null;
        static GameObject InvisGreenScreenOBJ;
        static GameObject Line;
        static List<GameObject> lines = new List<GameObject>();

        static GameObject tempBrush = null;
        static GameObject tempDropper = null;

        static LineRenderer line;

        static float WipeT = 3;

        static AssetBundle bundle;
        static AssetBundle bundle2;

        static Vector3 GSP;
        static Vector3 PrePos;

        static float MinDst = 0.01f;
        public static AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        void Start()
        {
            bundle = LoadAssetBundle("Green_Screen_Mod.gtagbrush");
            bundle2 = LoadAssetBundle("Green_Screen_Mod.gtagdropper");
        }

        void FixedUpdate()
        {
            //  Debug.Log("List");
            //  List<InputDevice> list = new List<InputDevice>(1);
            //  Debug.Log("List2");
            //  InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, list);
            //  Debug.Log("List3");
            //  list[1].TryGetFeatureValue(CommonUsages.gripButton, out gripDown);
            // list[1].TryGetFeatureValue(CommonUsages.triggerButton, out triggerDown);
            //list[1].TryGetFeatureValue(CommonUsages.primary2DAxis, out ThumbStickR);
            //list[1].TryGetFeatureValue(CommonUsages.primaryButton, out primaryDown);
            try
            {
                //Controlls the whole tool selection
                if (InputController.instance.LSecondaryDown)
                {
                    //wipes old tool
                    if (!InputController.instance.TempWiping)
                    {
                        InputController.instance.TempWiping = true;
                        GameObject.Destroy(PaintPallet.instance.Painter);
                        GameObject.Destroy(GreenScreenOBJ);
                        GameObject.Destroy(InvisGreenScreenOBJ);
                        GameObject.Destroy(Dropper);
                        PaintPallet.instance.Painter = null;
                        GreenScreenOBJ = null;
                        InvisGreenScreenOBJ = null;
                        Dropper = null;
                        PaintPallet.instance.LineW = 0.1f;
                        InputController.instance.Drawing = true;
                        InputController.instance.Dropping = false;

                    }
                    //spawns a temp brush
                    if (tempBrush == null)
                    {
                        tempBrush = Instantiate(bundle.LoadAsset<GameObject>("GtagBrush"));
                        tempBrush.transform.parent = GorillaLocomotion.Player.Instance.leftControllerTransform;
                        tempBrush.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                        tempBrush.transform.localPosition = new Vector3(0f, 0.1f, 0f);
                        tempBrush.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                        tempBrush.transform.localScale = new Vector3(0.012f, 0.012f, 0.012f);
                        tempBrush.transform.Find("Brush").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
                        tempBrush.transform.Find("Handle").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
                        tempBrush.transform.Find("Brush").GetComponent<Renderer>().material.color = new Color(0, 0, 1,0.5f);
                        tempBrush.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                    }
                    //spawns a temp dropper
                    if (tempDropper == null)
                    {
                        tempDropper = Instantiate(bundle2.LoadAsset<GameObject>("GtagDropper"));
                        tempDropper.transform.parent = GorillaLocomotion.Player.Instance.leftControllerTransform;
                        tempDropper.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                        tempDropper.transform.localPosition = new Vector3(0f, -0.25f, 0f);
                        tempDropper.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                        tempDropper.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                        tempDropper.transform.Find("Tip").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
                        tempDropper.transform.Find("Handle").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
                        tempDropper.transform.Find("Tip").GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                        tempDropper.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                    }
                    //checks if the hand is pointing toward the brush and changes size and color
                    if (InputController.instance.CRY <= -0.15)
                    {
                        tempBrush.transform.localScale = new Vector3(0.020f, 0.020f, 0.020f);
                        tempBrush.transform.Find("Brush").GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 1, 0.5f);
                        tempBrush.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 1, 0.5f);
                    }
                    //checks if the hand is pointing toward the dropper and changes size and color
                    else if (InputController.instance.CRY >= 0.15)
                    {
                        tempDropper.transform.localScale = new Vector3(0.10f, 0.10f, 0.10f);
                        tempDropper.transform.Find("Tip").GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 1, 0.5f);
                        tempDropper.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 1, 0.5f);
                    }
                    //if not pointing towards any keep old colors
                    else
                    {
                        tempBrush.transform.localScale = new Vector3(0.012f, 0.012f, 0.012f);
                        tempDropper.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                        tempBrush.transform.Find("Brush").GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                        tempBrush.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                        tempDropper.transform.Find("Tip").GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                        tempDropper.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.5f);
                    }
                }
                //when released set to selected tool
                else if (!InputController.instance.LSecondaryDown && tempBrush != null && tempDropper != null)
                {
                    InputController.instance.TempWiping = false;
                    //draws the paint pallet
                    if (PaintPallet.instance.Painter == null)
                    {
                        PaintPallet.instance.DrawPainter();
                    }
                    //draws the brush
                    if (InputController.instance.CRY <= -0.3)
                    {
                        DrawGreenScreen();
                        InputController.instance.Dropping = false;
                        InputController.instance.Drawing = true;
                    }

                    //draws the dropper
                    else if (InputController.instance.CRY >= 0.3)
                    {
                        DrawDropper();
                        InputController.instance.Dropping = true;
                        InputController.instance.Drawing = false;
                    }
                    //removes the temp brush and dropper
                    GameObject.Destroy(tempBrush);
                    GameObject.Destroy(tempDropper);
                    tempBrush = null;
                    tempDropper = null;
                }
               if (GreenScreenOBJ != null && InputController.instance.Drawing)
                {
                    // shoots a raycast to keep the brush on whatever its hitting
                    if (Physics.Linecast(new Vector3(GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.x, GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.y + 0.5f, GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.z), InvisGreenScreenOBJ.transform.position, out RaycastHit info))
                    {
                        GSP = info.point;
                    }
                    else
                    {
                        GSP = InvisGreenScreenOBJ.transform.position;
                    }
                    //moves the position of the brush with the movement of the right joistick
                    InvisGreenScreenOBJ.transform.localPosition = new Vector3(InvisGreenScreenOBJ.transform.localPosition.x, InvisGreenScreenOBJ.transform.localPosition.y, InvisGreenScreenOBJ.transform.localPosition.z + InputController.instance.GSY * 0.1f);
                    GreenScreenOBJ.transform.position = GSP;
                    //draws the lines when trigger is pressed
                    if (InputController.instance.triggerDown && !InputController.instance.gripDown)
                    {
                        MakeLine();
                        DrawLine();

                    }
                    //resets the part that spawns a new line renderer when trigger is first pressed And adds a trigger collider
                    if (!InputController.instance.triggerDown & InputController.instance.TriggerToggle)
                    {
                        InputController.instance.TriggerToggle = false;
                        LineCollider.GenerateCollider(line);
                    }
                    //wipes the last line that was created
                    if (InputController.instance.primaryDown && !InputController.instance.Wiping)
                    {
                        InputController.instance.Wiping = true;
                        Debug.Log(InputController.instance.Wiping);
                        GameObject.Destroy(lines[lines.Count - 1]);
                        lines.RemoveAt(lines.Count - 1);
                        Debug.Log(lines);
                    }
                    else if (!InputController.instance.primaryDown && InputController.instance.Wiping)
                    {
                        InputController.instance.Wiping = false;
                    }
                    //wipes everything if wipe button is held for three seconds
                    if (InputController.instance.primaryDown && WipeT > 0)
                    {
                        WipeT -= Time.deltaTime;
                        if (InputController.instance.primaryDown && WipeT <= 0)
                        {
                            foreach (var line in lines)
                            {
                                GameObject.Destroy(line.gameObject);
                                Debug.Log(lines);
                            }
                            lines.Clear();
                        }
                    }
                    else if (!InputController.instance.primaryDown && WipeT <= 0)
                    {
                        WipeT = 3;
                    }
                     
                    //freezes the position of the brush
                    if (InputController.instance.LPrimaryDown && !InputController.instance.LPrimaryToggle)
                    {
                        InputController.instance.LPrimaryToggle = true;
                        if (!InputController.instance.FreezeBrush)
                        {
                            InputController.instance.FreezeBrush = true;
                            InputController.instance.GSY = 0;
                        }
                        else
                        {
                            InputController.instance.FreezeBrush = false;
                        }
                    }
                    else if (!InputController.instance.LPrimaryDown && InputController.instance.LPrimaryToggle)
                    {
                        InputController.instance.LPrimaryToggle = false;
                    }
                }
               //if not drawing destroys the brush
                else if (!InputController.instance.Drawing && GreenScreenOBJ != null)
                {
                    GameObject.Destroy(GreenScreenOBJ);
                    GreenScreenOBJ = null;
                    GameObject.Destroy(InvisGreenScreenOBJ);
                    InvisGreenScreenOBJ= null;
                }

                //destroys brush and paint pallet 
                if (InputController.instance.SecondaryToggle && !InputController.instance.DeleteWiping)
                {
                    InputController.instance.DeleteWiping = true;
                    GameObject.Destroy(PaintPallet.instance.Painter);
                    GameObject.Destroy(GreenScreenOBJ);
                    GameObject.Destroy(InvisGreenScreenOBJ);
                    GameObject.Destroy(Dropper);
                    PaintPallet.instance.Painter = null;
                    GreenScreenOBJ = null;
                    InvisGreenScreenOBJ = null;
                    Dropper = null;
                    PaintPallet.instance.LineW = 0.1f;
                    InputController.instance.Drawing = true;
                    InputController.instance.Dropping = false;
                }
                else if(!InputController.instance.SecondaryToggle && InputController.instance.DeleteWiping)
                {
                    InputController.instance.DeleteWiping = false;
                }
                //controlls the dropper
                if (InputController.instance.Dropping)
                {
                    //draws the dropper
                    if (Dropper == null)
                    {
                        DrawDropper();
                    }
                    else if (Dropper != null)
                    {
                        //shoots a raycast out from players hand to what you wana drop
                        if (Physics.Raycast(new Vector3(GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.x, GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.y + 0.5f, GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.z), GorillaLocomotion.Player.Instance.rightControllerTransform.transform.forward, out RaycastHit info))
                        {
                            //sets droppers position to the hit point and sets droppers color
                            Dropper.transform.position = info.point;
                            Dropper.transform.Find("Tip").GetComponent<Renderer>().material.color = info.transform.GetComponent<Renderer>().material.color;
                            //if trigger hit then sets the paint pallets rgb values to the rounded values of what was hit and redraws the painter
                            if(InputController.instance.triggerDown && !InputController.instance.DropperToggle)
                            {
                                InputController.instance.DropperToggle = true;
                                PaintPallet.instance.R = Mathf.Round(info.transform.GetComponent<Renderer>().material.color.r * 10.0f) * 0.1f;
                                PaintPallet.instance.B = Mathf.Round(info.transform.GetComponent<Renderer>().material.color.b * 10.0f) * 0.1f;
                                PaintPallet.instance.G = Mathf.Round(info.transform.GetComponent<Renderer>().material.color.g * 10.0f) * 0.1f;
                                GameObject.Destroy(PaintPallet.instance.Painter);
                                PaintPallet.instance.Painter = null;
                                PaintPallet.instance.LineW = 0.1f;
                                PaintPallet.instance.DrawPainter();
                            }
                            if(!InputController.instance.triggerDown && InputController.instance.DropperToggle)
                            {
                                InputController.instance.DropperToggle = false;
                            }
                        }
                    }
                }
                //if not dropping destoys the dropper
                else if (!InputController.instance.Dropping && Dropper != null)
                {
                    GameObject.Destroy(Dropper);
                    Dropper = null;
                }
                //adds a cooldown to the buttons
                if (BtnCooldown > 0)
                {
                    if (Time.frameCount > BtnCooldown)
                    {
                        BtnCooldown = 0;
                        GameObject.Destroy(PaintPallet.instance.Painter);
                        PaintPallet.instance.Painter = null;

                        PaintPallet.instance.DrawPainter();
                    }
                }
            }
            catch
            {

            }

        }

        static void MakeLine()
        {
            //Creates game object with line renderer
            if (InputController.instance.triggerDown & !InputController.instance.TriggerToggle)
            {
                Debug.Log("LineMade");
                InputController.instance.TriggerToggle = true;
                Line = new GameObject();
                Line.AddComponent<LineRenderer>();
                Line.layer = 18;
                line = Line.GetComponent<LineRenderer>();
                PrePos = Line.transform.position;
                line.positionCount = 1;
                line.numCornerVertices = 90;
                line.numCapVertices = 90;
                line.sortingOrder = 1;
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.material.color = new Color(PaintPallet.instance.R, PaintPallet.instance.G, PaintPallet.instance.B);
                line.startColor = line.endColor = new Color(PaintPallet.instance.R, PaintPallet.instance.G, PaintPallet.instance.B);
                line.startWidth = line.endWidth = PaintPallet.instance.LineW;
                lines.Add(Line);
            }
        }

        static void DrawLine()
        {
            //adds all the points in the line renderer and draws the lines
            if (InputController.instance.triggerDown && !InputController.instance.gripDown)
            {
                Vector3 CurPos = GreenScreenOBJ.transform.position;

                if (Vector3.Distance(CurPos, PrePos) > MinDst)
                {
                    //May break here
                    Debug.Log(PrePos + "PrePos");
                    Debug.Log(CurPos + "CurPos");
                    Debug.Log(GreenScreenOBJ.transform.position + "GreenScreenOBJ.transform.position");
                    if (PrePos == Line.transform.position)
                    {
                        line.SetPosition(0, CurPos);
                        Debug.Log("ResetPos");
                    }
                    else
                    {
                        line.positionCount++;
                        line.SetPosition(line.positionCount - 1, CurPos);
                    }
                    PrePos = CurPos;
                }
            }
        }

        public static void DrawGreenScreen()
        {
            //draws the brush and the invisible point that the raycast is casted to.

            GreenScreenOBJ = Instantiate(bundle.LoadAsset<GameObject>("GtagBrush"));
            GreenScreenOBJ.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
            GreenScreenOBJ.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            GreenScreenOBJ.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
            GreenScreenOBJ.transform.localScale = new Vector3(PaintPallet.instance.LineW * 0.1f, PaintPallet.instance.LineW * 0.1f, PaintPallet.instance.LineW * 0.1f);
            GreenScreenOBJ.transform.Find("Brush").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            GreenScreenOBJ.transform.Find("Handle").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            GreenScreenOBJ.transform.Find("Brush").GetComponent<Renderer>().material.color = new Color(PaintPallet.instance.R, PaintPallet.instance.G, PaintPallet.instance.B);
            GreenScreenOBJ.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(0, 0, 1);
            GreenScreenOBJ.layer = LayerMask.NameToLayer("Ignore Raycast");

            //invis point for raycast
            if (InvisGreenScreenOBJ)
                GameObject.Destroy(InvisGreenScreenOBJ);

            InvisGreenScreenOBJ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            InvisGreenScreenOBJ.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
            InvisGreenScreenOBJ.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            InvisGreenScreenOBJ.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
            InvisGreenScreenOBJ.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            InvisGreenScreenOBJ.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            InvisGreenScreenOBJ.GetComponent<Renderer>().enabled = false;
            InvisGreenScreenOBJ.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(InvisGreenScreenOBJ.GetComponent<Rigidbody>());
            GameObject.Destroy(InvisGreenScreenOBJ.GetComponent<Collider>());
        }

        public static void DrawDropper()
        {
            //draws the dropper
            Debug.Log(Dropper);
            Dropper = Instantiate(bundle2.LoadAsset<GameObject>("GtagDropper"));
            Dropper.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
            Dropper.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            Dropper.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
            Dropper.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Dropper.transform.Find("Tip").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            Dropper.transform.Find("Handle").GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            Dropper.transform.Find("Tip").GetComponent<Renderer>().material.color = new Color(1, 1, 1);
            Dropper.transform.Find("Handle").GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            Dropper.layer = LayerMask.NameToLayer("Ignore Raycast");

            Debug.Log(Dropper);
        }

    }

}