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
using Green_Screen_Mod_Gtag;

namespace Green_Screen_Mod
{
    class PaintPallet : MonoBehaviour
    {
        public static PaintPallet instance;

        public float R = 0;
        public float G = 1;
        public float B = 0;
        public float LineW = 0.1f;

        public AssetBundle bundle;

        public GameObject Painter = null;

        public GameObject button1;
        public GameObject button2;
        public GameObject button3;
        public GameObject button4;
        public GameObject button5;
        public GameObject button6;
        public GameObject button7;
        public GameObject button8;

        public GameObject color1;
        public GameObject color2;
        public GameObject color3;
        public GameObject colorAll;
        public GameObject LineWidth;

        public String[] buttons = new String[]
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
        public AssetBundle LoadAssetBundle(string path)
        {
            //method that loads custom model
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
        void Start()
        {
            //loads custom model
            PaintPallet.instance = this;
            bundle = LoadAssetBundle("Green_Screen_Mod.gtagpainter");
        }

        public void DrawPainter()
        {
            // spawns the painting pallet
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

            //canvasOBJ = new GameObject();
            //canvasOBJ.transform.parent = Painter.transform;
            //Canvas canvas = canvasOBJ.AddComponent<Canvas>();
            //CanvasScaler canvasScale = canvasOBJ.AddComponent<CanvasScaler>();
            //canvasOBJ.AddComponent<GraphicRaycaster>();
            //canvas.renderMode = RenderMode.WorldSpace;
            //canvasScale.dynamicPixelsPerUnit = 1000;

            //spawns the buttons and their indicators

            DrawButtons();
            DrawButtonActivates();
        }

        void DrawButtons()
        {
            //draws all buttons in a specific location (this probably is going to be changed in the future thats why they all are drawn seperatly)
            button1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button1.GetComponent<Collider>().isTrigger = true;
            button1.transform.parent = Painter.transform;
            button1.transform.localScale = new Vector3(.25f, .25f, .25f);
            button1.transform.rotation = Painter.transform.rotation;
            button1.transform.localPosition = new Vector3(-0.8f, 0.6f, -1.2f);
            button1.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button1.GetComponent<Renderer>().material.color = Color.white;
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
            button2.GetComponent<Collider>().isTrigger = true;
            button2.transform.parent = Painter.transform;
            button2.transform.localScale = new Vector3(.25f, .25f, .25f);
            button2.transform.rotation = Painter.transform.rotation;
            button2.transform.localPosition = new Vector3(-0.8f, 0.6f, -0.3f);
            button2.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button2.GetComponent<Renderer>().material.color = Color.white;
            button2.layer = 18;
            button2.AddComponent<ButtonCollision>().BtnId = buttons[1];

            GameObject.Destroy(button2.GetComponent<Rigidbody>());

            button3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button3.GetComponent<Collider>().isTrigger = true;
            button3.transform.parent = Painter.transform;
            button3.transform.localScale = new Vector3(.25f, .25f, .25f);
            button3.transform.rotation = Painter.transform.rotation;
            button3.transform.localPosition = new Vector3(-0.3f, 0.6f, -1.2f);
            button3.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button3.GetComponent<Renderer>().material.color = Color.white;
            button3.layer = 18;
            button3.AddComponent<ButtonCollision>().BtnId = buttons[2];

            GameObject.Destroy(button3.GetComponent<Rigidbody>());

            button4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button4.GetComponent<Collider>().isTrigger = true;
            button4.transform.parent = Painter.transform;
            button4.transform.localScale = new Vector3(.25f, .25f, .25f);
            button4.transform.rotation = Painter.transform.rotation;
            button4.transform.localPosition = new Vector3(-0.3f, 0.6f, -0.3f);
            button4.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button4.GetComponent<Renderer>().material.color = Color.white;
            button4.layer = 18;
            button4.AddComponent<ButtonCollision>().BtnId = buttons[3];

            GameObject.Destroy(button4.GetComponent<Rigidbody>());

            button5 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button5.GetComponent<Collider>().isTrigger = true;
            button5.transform.parent = Painter.transform;
            button5.transform.localScale = new Vector3(.25f, .25f, .25f);
            button5.transform.rotation = Painter.transform.rotation;
            button5.transform.localPosition = new Vector3(0.2f, 0.6f, -1.2f);
            button5.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button5.GetComponent<Renderer>().material.color = Color.white;
            button5.layer = 18;
            button5.AddComponent<ButtonCollision>().BtnId = buttons[4];

            GameObject.Destroy(button5.GetComponent<Rigidbody>());

            button6 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button6.GetComponent<Collider>().isTrigger = true;
            button6.transform.parent = Painter.transform;
            button6.transform.localScale = new Vector3(.25f, .25f, .25f);
            button6.transform.rotation = Painter.transform.rotation;
            button6.transform.localPosition = new Vector3(0.2f, 0.6f, -0.3f);
            button6.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button6.GetComponent<Renderer>().material.color = Color.white;
            button6.layer = 18; ;
            button6.AddComponent<ButtonCollision>().BtnId = buttons[5];

            GameObject.Destroy(button6.GetComponent<Rigidbody>());

            button7 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button7.GetComponent<Collider>().isTrigger = true;
            button7.transform.parent = Painter.transform;
            button7.transform.localScale = new Vector3(.25f, .25f, .25f);
            button7.transform.rotation = Painter.transform.rotation;
            button7.transform.localPosition = new Vector3(0.7f, 0.6f, -1.2f);
            button7.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button7.GetComponent<Renderer>().material.color = Color.white;
            button7.layer = 18;
            button7.AddComponent<ButtonCollision>().BtnId = buttons[6];

            GameObject.Destroy(button7.GetComponent<Rigidbody>());

            button8 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button8.GetComponent<Collider>().isTrigger = true;
            button8.transform.parent = Painter.transform;
            button8.transform.localScale = new Vector3(.25f, .25f, .25f);
            button8.transform.rotation = Painter.transform.rotation;
            button8.transform.localPosition = new Vector3(0.7f, 0.6f, -0.3f);
            button8.GetComponent<Renderer>().material.shader = GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].shader;
            button8.GetComponent<Renderer>().material.color = Color.white;
            button8.layer = 18;
            button8.AddComponent<Text>();
            button8.AddComponent<ButtonCollision>().BtnId = buttons[7];

            GameObject.Destroy(button8.GetComponent<Rigidbody>());

        }

        void DrawButtonActivates()
        {
            //draws all button indicators
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

        public void TriggerBtns(String BtnId)
        {
            //finds if what was pressed was one of the buttons
            Debug.Log(BtnId);
            int ButtonNum = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (BtnId == buttons[i])
                {
                    ButtonNum = i;
                    break;
                }
            }
            //takes the button pressed and activates what it does
            Debug.Log(ButtonNum);
            if (ButtonNum == 0)
            {
                //red down
                Debug.Log("pressed0");
                if (R >= 0.1)
                    R -= 0.1f;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }
            else if (ButtonNum == 1)
            {
                // red up
                Debug.Log("pressed1");
                if (R < 1)
                    R += 0.1f;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }
            if (ButtonNum == 2)
            {
                //green down
                Debug.Log("pressed2");
                if (G >= 0.1)
                    G -= 0.1f; ;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }
            else if (ButtonNum == 3)
            {
                //green up
                Debug.Log("pressed3");
                if (G < 1)
                    G += 0.1f;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }
            if (ButtonNum == 4)
            {
                //blue down
                Debug.Log("pressed4");
                if (B >= 0.1)
                    B -= 0.1f; ;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }
            else if (ButtonNum == 5)
            {
                //blue up
                Debug.Log("pressed5");
                if (B < 1)
                    B += 0.1f;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }
            if (ButtonNum == 6)
            {
                // line width down
                Debug.Log("pressed4");
                if (LineW >= 0.2)
                    LineW -= 0.1f; ;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }
            else if (ButtonNum == 7)
            {
                // line width up
                Debug.Log("pressed5");
                if (LineW < 1)
                    LineW += 0.1f;
                GameObject.Destroy(Painter);
                GameObject.Destroy(MainPatch.GreenScreenOBJ);
                DrawPainter();
                if (InputController.instance.Drawing)
                {
                    MainPatch.DrawGreenScreen();
                }
            }

        }
    }
    class ButtonCollision : MonoBehaviour
    {
        public string BtnId;
        private void OnTriggerEnter(Collider collider)
        {
            //whenever the buttons are pressed a cooldown starts and it starts the TriggerBtns method
            if (Time.frameCount >= MainPatch.FramePressCoolDown + 30)
            {
                PaintPallet.instance.TriggerBtns(BtnId);
                MainPatch.FramePressCoolDown = Time.frameCount;
            }

        }

    }
}