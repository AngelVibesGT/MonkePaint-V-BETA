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

namespace Green_Screen_Mod_Gtag
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Class1 : BaseUnityPlugin
    {

        private const string modGUID = "Green.Screen.Mod.By.AV";
        private const string modName = "MonkePaint V BETA ";
        private const string modVersion = "0.0.0.1";

        public void Awake()
        {
            var harmony = new Harmony(modGUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        static bool gripDown;
        static bool triggerDown;
        static bool primaryDown;
        static bool Wiping;
        static bool TriggerToggle;
        static bool SecondaryToggle;

        public static int FramePressCoolDown = 0;
        static int BtnCooldown;

        static GameObject GreenScreenOBJ;
        static GameObject InvisGreenScreenOBJ;
        static GameObject Line;
        static List<GameObject> lines = new List<GameObject>();
        static GameObject Painter = null;

        static GameObject button1;
        static GameObject button2;
        static GameObject button3;
        static GameObject button4;
        static GameObject button5;
        static GameObject button6;
        static GameObject button7;
        static GameObject button8;

        static GameObject color1;
        static GameObject color2;
        static GameObject color3;
        static GameObject colorAll;
        static GameObject LineWidth;

        static GameObject canvasOBJ = null;

        public static String[] buttons = new String[] 
        {
        "Red Down",
        "Red Up",
        "Green Down",
        "Green Up",
        "Blue Down",
        "Blue Up",
        "Line Up",
        "Line Down",
        };

        static LineRenderer line;

        static float GSY;
        static float LineW = 0.1f;

        public static float R = 0;
        public static float G = 1;
        public static float B = 0;

        static AssetBundle bundle;


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
            bundle = LoadAssetBundle("Green_Screen_Mod.gtagpainter");
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
                if (ControllerInputPoller.instance.rightControllerPrimaryButton)
                {
                    primaryDown = true;
                }
                else
                {
                    primaryDown = false;
                }
                if (ControllerInputPoller.instance.rightGrab)
                {
                    gripDown = true;
                }
                else
                {
                    gripDown = false;
                }
                GSY = ControllerInputPoller.instance.rightControllerPrimary2DAxis.y;


                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0)
                {
                    triggerDown = true;
                }
                else
                {
                    triggerDown = false;
                }
                if (ControllerInputPoller.instance.rightControllerSecondaryButton)
                {
                    SecondaryToggle = true;
                }
                else
                {
                    SecondaryToggle = false;
                }

                if (gripDown && triggerDown && Painter == null)
                {
                    DrawPainter();
                }
                else if (gripDown && triggerDown && Painter != null)
                {

                }

                if (gripDown && triggerDown)
                {
                    DrawGreenScreen();
                }
                else
                {
                    if (Physics.Linecast(new Vector3(GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.x, GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.y + 0.5f, GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position.z), InvisGreenScreenOBJ.transform.position, out RaycastHit info))
                    {
                        GSP = info.point;
                    }
                    else
                    {
                        GSP = InvisGreenScreenOBJ.transform.position;
                    }
                    InvisGreenScreenOBJ.transform.localPosition = new Vector3(InvisGreenScreenOBJ.transform.localPosition.x, InvisGreenScreenOBJ.transform.localPosition.y, InvisGreenScreenOBJ.transform.localPosition.z + GSY * 0.1f);
                    GreenScreenOBJ.transform.position = GSP;

                    if (triggerDown && !gripDown)
                    {
                        MakeLine();
                        DrawLine();

                    }
                    if (!triggerDown & TriggerToggle)
                    {
                        TriggerToggle = false;
                    }
                    if (primaryDown && !Wiping)
                    {
                        Wiping = true;
                        foreach (var line in lines)
                        {
                            GameObject.Destroy(line.gameObject);
                        }
                        Wiping = false;
                    }
                    if(SecondaryToggle && !Wiping)
                    {
                        Wiping = true;
                        GameObject.Destroy(Painter);
                        GameObject.Destroy(GreenScreenOBJ);
                        GameObject.Destroy(InvisGreenScreenOBJ);
                        Painter = null;
                        GreenScreenOBJ = null;
                        InvisGreenScreenOBJ = null;
                        LineW = 0.1f;

                        foreach (var line in lines)
                        {
                            GameObject.Destroy(line.gameObject);
                        }
                        Wiping = false;

                    }
                }
                if(BtnCooldown > 0)
                {
                    if(Time.frameCount > BtnCooldown)
                    {
                        BtnCooldown = 0;
                        GameObject.Destroy(Painter);
                        Painter = null;

                        DrawPainter();
                    }
                }
            }
            catch
            {

            }

        }

        static void MakeLine()
        {

            if (triggerDown & !TriggerToggle)
            {
                Debug.Log("LineMade");
                TriggerToggle = true;
                Line = new GameObject();
                Line.AddComponent<LineRenderer>();
                line = Line.GetComponent<LineRenderer>();
                PrePos = Line.transform.position;
                line.positionCount = 1;
                line.numCornerVertices = 90;
                line.numCapVertices = 90;
                line.sortingOrder = 1;
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.material.color = new Color(R, G, B);
                line.startColor = line.endColor = new Color(R, G, B);
                line.startWidth = line.endWidth = LineW;
                lines.Add(Line);
            }
        }

        static void DrawLine()
        {
            if (triggerDown && !gripDown)
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

        static void DrawGreenScreen()
        {
            if (GreenScreenOBJ)
                GameObject.Destroy(GreenScreenOBJ);

            GreenScreenOBJ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GreenScreenOBJ.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
            GreenScreenOBJ.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
            GreenScreenOBJ.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
            GreenScreenOBJ.transform.localScale = new Vector3(LineW, LineW, LineW);
            GreenScreenOBJ.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            GreenScreenOBJ.GetComponent<Renderer>().material.color = new Color(R, G, B);
            GreenScreenOBJ.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(GreenScreenOBJ.GetComponent<Rigidbody>());
            GameObject.Destroy(GreenScreenOBJ.GetComponent<Collider>());

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
        static void DrawPainter()
        {
            Debug.Log("Broke At Creation");
            Painter = Instantiate(bundle.LoadAsset<GameObject>("GtagPaint"));
            Debug.Log(Painter + " " + Painter.transform.position);
            Painter.transform.parent = GorillaLocomotion.Player.Instance.leftControllerTransform;
            Painter.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
            Painter.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
            Painter.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            Painter.GetComponentInChildren<Renderer>().material = new Material(GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader);
            Painter.GetComponentInChildren<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            Painter.GetComponentInChildren<Renderer>().material.color = new Color(0.6f, 0.3f, 0);
            Painter.layer = LayerMask.NameToLayer("Ignore Raycast");
            GameObject.Destroy(Painter.GetComponent<Rigidbody>());
            GameObject.Destroy(Painter.GetComponent<Collider>());

            canvasOBJ = new GameObject();
            canvasOBJ.transform.parent = Painter.transform;
            Canvas canvas = canvasOBJ.AddComponent<Canvas>();
            CanvasScaler canvasScale = canvasOBJ.AddComponent<CanvasScaler>();
            canvasOBJ.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScale.dynamicPixelsPerUnit = 1000;

            DrawButtons();
            DrawButtonActivates();
        }

        static void DrawButtons()
        {
            button1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button1.transform.parent = Painter.transform;
            button1.transform.localScale = new Vector3(.25f, .25f, .25f);
            button1.transform.rotation = Painter.transform.rotation;
            button1.transform.localPosition = new Vector3(-0.8f, 0.6f, -1.2f);
            button1.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button1.GetComponent<Renderer>().material.color = Color.white;
            button1.GetComponent<Collider>().isTrigger = true;
            button1.layer = 18;
            button1.AddComponent<ButtonCollision>().BtnId = buttons[0];

            /* GameObject titleOBJ = new GameObject();
             titleOBJ.transform.parent = canvasOBJ.transform;
             Text title = titleOBJ.AddComponent<Text>();
             title.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
             title.fontSize = 1;
             title.alignment = TextAnchor.MiddleCenter;
             title.resizeTextForBestFit = true;
             title.resizeTextMinSize = 0;
             title.text = "<";
             RectTransform titleTransform = title.GetComponent<RectTransform>();
             titleTransform.localPosition = Vector3.zero;
             titleTransform.sizeDelta = new Vector2(.2f, 0.03f);
             titleTransform.position = new Vector3(1.2f, -1.1f, 0.8f);
             titleTransform.rotation = Quaternion.Euler(new Vector3(180, 90, 90));
            */
            GameObject.Destroy(button1.GetComponent<Rigidbody>());

            button2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button2.transform.parent = Painter.transform;
            button2.transform.localScale = new Vector3(.25f, .25f, .25f);
            button2.transform.rotation = Painter.transform.rotation;
            button2.transform.localPosition = new Vector3(-0.8f, 0.6f, -0.3f);
            button2.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button2.GetComponent<Renderer>().material.color = Color.white;
            button2.GetComponent<Collider>().isTrigger = true;
            button2.layer = 18;
            button2.AddComponent<ButtonCollision>().BtnId = buttons[1];

            GameObject.Destroy(button2.GetComponent<Rigidbody>());

            button3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button3.transform.parent = Painter.transform;
            button3.transform.localScale = new Vector3(.25f, .25f, .25f);
            button3.transform.rotation = Painter.transform.rotation;
            button3.transform.localPosition = new Vector3(-0.3f, 0.6f, -1.2f);
            button3.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button3.GetComponent<Renderer>().material.color = Color.white;
            button3.GetComponent<Collider>().isTrigger = true;
            button3.layer = 18;
            button3.AddComponent<ButtonCollision>().BtnId = buttons[2];

            GameObject.Destroy(button3.GetComponent<Rigidbody>());

            button4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button4.transform.parent = Painter.transform;
            button4.transform.localScale = new Vector3(.25f, .25f, .25f);
            button4.transform.rotation = Painter.transform.rotation;
            button4.transform.localPosition = new Vector3(-0.3f, 0.6f, -0.3f);
            button4.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button4.GetComponent<Renderer>().material.color = Color.white;
            button4.GetComponent<Collider>().isTrigger = true;
            button4.layer = 18;
            button4.AddComponent<ButtonCollision>().BtnId = buttons[3];

            GameObject.Destroy(button4.GetComponent<Rigidbody>());

            button5 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button5.transform.parent = Painter.transform;
            button5.transform.localScale = new Vector3(.25f, .25f, .25f);
            button5.transform.rotation = Painter.transform.rotation;
            button5.transform.localPosition = new Vector3(0.2f, 0.6f, -1.2f);
            button5.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button5.GetComponent<Renderer>().material.color = Color.white;
            button5.GetComponent<Collider>().isTrigger = true;
            button5.layer = 18;
            button5.AddComponent<ButtonCollision>().BtnId = buttons[4];

            GameObject.Destroy(button5.GetComponent<Rigidbody>());

            button6 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button6.transform.parent = Painter.transform;
            button6.transform.localScale = new Vector3(.25f, .25f, .25f);
            button6.transform.rotation = Painter.transform.rotation;
            button6.transform.localPosition = new Vector3(0.2f, 0.6f, -0.3f);
            button6.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button6.GetComponent<Renderer>().material.color = Color.white;
            button6.GetComponent<Collider>().isTrigger = true;
            button6.layer = 18; ;
            button6.AddComponent<ButtonCollision>().BtnId = buttons[5];

            GameObject.Destroy(button6.GetComponent<Rigidbody>());

            button7 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button7.transform.parent = Painter.transform;
            button7.transform.localScale = new Vector3(.25f, .25f, .25f);
            button7.transform.rotation = Painter.transform.rotation;
            button7.transform.localPosition = new Vector3(0.7f, 0.6f, -1.2f);
            button7.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button7.GetComponent<Renderer>().material.color = Color.white;
            button7.GetComponent<Collider>().isTrigger = true;
            button7.layer = 18;
            button7.AddComponent<ButtonCollision>().BtnId = buttons[6];

            GameObject.Destroy(button7.GetComponent<Rigidbody>());

            button8 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button8.transform.parent = Painter.transform;
            button8.transform.localScale = new Vector3(.25f, .25f, .25f);
            button8.transform.rotation = Painter.transform.rotation;
            button8.transform.localPosition = new Vector3(0.7f, 0.6f, -0.3f);
            button8.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button8.GetComponent<Renderer>().material.color = Color.white;
            button8.GetComponent<Collider>().isTrigger = true;
            button8.layer = 18;
            button8.AddComponent<Text>();
            button8.AddComponent<ButtonCollision>().BtnId = buttons[7];

            GameObject.Destroy(button8.GetComponent<Rigidbody>());

        }

        static void DrawButtonActivates()
        {

            color1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            color1.transform.parent = Painter.transform;
            color1.transform.localScale = new Vector3(.2f, .2f, .2f);
            color1.transform.rotation = Painter.transform.rotation;
            color1.transform.localPosition = new Vector3(-0.8f, 0.6f, -0.75f);
            color1.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            color1.GetComponent<Renderer>().material.color = new Color(R, 0, 0);
            color1.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(color1.GetComponent<Rigidbody>());
            GameObject.Destroy(color1.GetComponent<Collider>());


            color2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            color2.transform.parent = Painter.transform;
            color2.transform.localScale = new Vector3(.2f, .2f, .2f);
            color2.transform.rotation = Painter.transform.rotation;
            color2.transform.localPosition = new Vector3(-0.3f, 0.6f, -0.75f);
            color2.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            color2.GetComponent<Renderer>().material.color = new Color(0, G, 0);
            color2.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(color2.GetComponent<Rigidbody>());
            GameObject.Destroy(color2.GetComponent<Collider>());


            color3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            color3.transform.parent = Painter.transform;
            color3.transform.localScale = new Vector3(.2f, .2f, .2f);
            color3.transform.rotation = Painter.transform.rotation;
            color3.transform.localPosition = new Vector3(0.2f, 0.6f, -0.75f);
            color3.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            color3.GetComponent<Renderer>().material.color = new Color(0, 0, B);
            color3.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(color3.GetComponent<Rigidbody>());
            GameObject.Destroy(color3.GetComponent<Collider>());

            LineWidth = GameObject.CreatePrimitive(PrimitiveType.Cube);
            LineWidth.transform.parent = Painter.transform;
            LineWidth.transform.localScale = new Vector3(LineW, .2f, .2f);
            LineWidth.transform.rotation = Painter.transform.rotation;
            LineWidth.transform.localPosition = new Vector3(0.7f, 0.6f, -0.75f);
            LineWidth.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            LineWidth.GetComponent<Renderer>().material.color = new Color(R, G, B);
            LineWidth.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(LineWidth.GetComponent<Rigidbody>());
            GameObject.Destroy(LineWidth.GetComponent<Collider>());

            colorAll = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colorAll.transform.parent = Painter.transform;
            colorAll.transform.localScale = new Vector3(.2f, .2f, .2f);
            colorAll.transform.rotation = Painter.transform.rotation;
            colorAll.transform.localPosition = new Vector3(-0.8f, 0.6f, 0.7f);
            colorAll.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            colorAll.GetComponent<Renderer>().material.color = new Color(R, G, B);
            colorAll.layer = LayerMask.NameToLayer("Ignore Raycast");

            GameObject.Destroy(colorAll.GetComponent<Rigidbody>());
            GameObject.Destroy(colorAll.GetComponent<Collider>());
        }

        public static void TriggerBtns(String BtnId)
        {
            Debug.Log(BtnId);
            int index = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (BtnId == buttons[i])
                {
                    index = i;
                    break;
                }
            }
            Debug.Log(index);
                    if (index == 0)
                    {
                        Debug.Log("pressed0");
                        if (R >= 0.1)
                            R -= 0.1f;
                GameObject.Destroy(Painter);
                    DrawPainter();
                DrawGreenScreen();
                    }
                    else if (index == 1)
                    {
                        Debug.Log("pressed1");
                        if (R < 1)
                            R += 0.1f;
                GameObject.Destroy(Painter);
                DrawPainter();
                DrawGreenScreen();
            }
                    if (index == 2)
                    {
                        Debug.Log("pressed2");
                        if (G >= 0.1)
                            G -= 0.1f; ;
                GameObject.Destroy(Painter);
                DrawPainter();
                DrawGreenScreen();
            }
                    else if (index == 3)
                    {
                        Debug.Log("pressed3");
                        if (G < 1)
                            G += 0.1f;
                GameObject.Destroy(Painter);
                DrawPainter();
                DrawGreenScreen();
            }
                    if (index == 4)
                    {
                        Debug.Log("pressed4");
                        if (B >= 0.1)
                            B -= 0.1f; ;
                GameObject.Destroy(Painter);
                DrawPainter();
                DrawGreenScreen();
            }
                    else if (index == 5)
                    {
                        Debug.Log("pressed5");
                        if (B < 1)
                            B += 0.1f;
                GameObject.Destroy(Painter);
                DrawPainter();
                DrawGreenScreen();
            }
            if (index == 6)
            {
                Debug.Log("pressed4");
                if (LineW >= 0.2)
                    LineW -= 0.1f; ;
                GameObject.Destroy(Painter);
                DrawPainter();
                DrawGreenScreen();
            }
            else if (index == 7)
            {
                Debug.Log("pressed5");
                if (LineW < 1)
                    LineW += 0.1f;
                GameObject.Destroy(Painter);
                DrawPainter();
                DrawGreenScreen();
            }

        }
    }
        class ButtonCollision : MonoBehaviour
        {
        public string BtnId;
            private void OnTriggerEnter(Collider collider)
            {
            if(Time.frameCount >= Class1.FramePressCoolDown + 30)
            {
                Class1.TriggerBtns(BtnId);
                Class1.FramePressCoolDown = Time.frameCount;
            }
                
            }

        }

}