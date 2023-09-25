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

        static GameObject GreenScreenOBJ;
        static GameObject InvisGreenScreenOBJ;
        static GameObject Line;
        static List<GameObject> lines = new List<GameObject>();

        static GameObject canvasOBJ = null;

        static LineRenderer line;

        static float WipeT = 3;


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
                //draws Paint Pallet
                if (InputController.instance.gripDown && InputController.instance.triggerDown && PaintPallet.instance.Painter == null)
                {
                    Debug.Log("Drawing Painter");
                    PaintPallet.instance.DrawPainter();
                }
                else if (InputController.instance.gripDown && InputController.instance.triggerDown && PaintPallet.instance.Painter != null)
                {
                    Debug.Log(InputController.instance.gripDown);
                }
                //draws brush
                if (InputController.instance.gripDown && InputController.instance.triggerDown)
                {
                    Debug.Log("Drawing GS");
                    DrawGreenScreen();
                }
                else
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
                    //resets the part that spawns a new line renderer when trigger is first pressed
                    if (!InputController.instance.triggerDown & InputController.instance.TriggerToggle)
                    {
                        InputController.instance.TriggerToggle = false;
                    }
                    //wipes the last line that was created
                    if (InputController.instance.primaryDown && !InputController.instance.Wiping)
                    {
                        InputController.instance.Wiping = true;
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
                    //destroys brush and paint pallet 
                    if (InputController.instance.SecondaryToggle && !InputController.instance.Wiping)
                    {
                        InputController.instance.Wiping = true;
                        GameObject.Destroy(PaintPallet.instance.Painter);
                        GameObject.Destroy(GreenScreenOBJ);
                        GameObject.Destroy(InvisGreenScreenOBJ);
                        PaintPallet.instance.Painter = null;
                        GreenScreenOBJ = null;
                        InvisGreenScreenOBJ = null;
                        PaintPallet.instance.LineW = 0.1f;

                        //foreach (var line in lines)
                        //{
                        // GameObject.Destroy(line.gameObject);
                        //}
                        InputController.instance.Wiping = false;

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
            //draws the brush and the invisible point that the raycast is cated to.
            if (GreenScreenOBJ)
                GameObject.Destroy(GreenScreenOBJ);

            GreenScreenOBJ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GreenScreenOBJ.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
            GreenScreenOBJ.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            GreenScreenOBJ.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
            GreenScreenOBJ.transform.localScale = new Vector3(PaintPallet.instance.LineW, PaintPallet.instance.LineW, PaintPallet.instance.LineW);
            GreenScreenOBJ.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            GreenScreenOBJ.GetComponent<Renderer>().material.color = new Color(PaintPallet.instance.R, PaintPallet.instance.G, PaintPallet.instance.B);
            GreenScreenOBJ.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(GreenScreenOBJ.GetComponent<Rigidbody>());
            GameObject.Destroy(GreenScreenOBJ.GetComponent<Collider>());
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

    }

}