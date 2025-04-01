using System.Collections.Generic;
/// <summary>
/// Create by vvvision ,got help from 
/// http://vvvision.net/zblog/post/MultiProjectionUnity.html
/// </summary>
/// 
using UnityEngine;

public class MP_MultiScreen : MonoBehaviour
{

    /// <summary>
    /// Screen Resolution for single screen you can define your own here.
    /// </summary>
    public enum ScreenResolution
    {
        S_800X600 = 0,
        S_1024X768,    //default
        S_1280X720,
        S_1280X800,
        S_1920X1080,
        S_1920X1200,
    };

    /// <summary>
    /// Screen Layout  is mean how many projector for blending
    /// </summary>
    public enum ScreenLayout
    {
        L_1X1 = 0, //single monitor use for 3d mapping
        L_2X1,   // 2*1 monitor in horizontal normally 2048*768 or 1920*2*1080
        L_3X1,   // 3*1 monitor in horizontal
        L_4X1,   // 4*1 monitor in horizontal
        L_5X1,   // 5*1 monitor in horizontal
        L_6X1,   // 6*1 monitor in horizontal
        L_7X1,   // 7*1 monitor in horizontal
        L_8X1,   // 8*1 monitor in horizontal
        L_2X2,   // 2*2 monitor as a squares 	
        L_2X3,   // 2*3 monitor as a squares 
        L_3X2,   // 3*2 monitor as a squares 
        L_2X4,   // 2*4 monitor as a squares 
        L_4X2,   // 4*2 monitor as a squares 
    };

    /// <summary>
    /// the overlap count of blend area, normally , 2 count is good , more count means more overlaping.
    /// </summary>
    public enum BlendAreaCount
    {
        B_0 = 0,  // no blend 
        B_1,
        B_2, //default
        B_3,
        B_4,
        B_5,       
    };

    /// <summary>
    /// detect what's object are ctrol , gird or color , that's use for share up/down key
    /// </summary>
    public enum CtrolObject
    {
        C_GRID = 0,  // grid ctrol now
        C_COLOR_R,   // color red
        C_COLOR_G,   // color green
        C_COLOR_B,   // color blue
    };

    public ScreenLayout layout = ScreenLayout.L_2X1;
    public BlendAreaCount blend_count = BlendAreaCount.B_2;
    public ScreenResolution screen_reslotion = ScreenResolution.S_1024X768;
    
    public GameObject screen_pre;
    private static MP_MultiScreen instance;
    public static MP_MultiScreen Instance { get { return instance; } private set { } }

    CtrolObject ctrol_object = CtrolObject.C_GRID;
    RenderTexture targetTexture;

    /// <summary>
    /// the screen gird size, you can change it as wish, more columns more smooth for grid and with more GPU task.
    /// </summary>
    public int COLUMNS = 16;
    public int ROWS = 12;

    /// <summary>
    /// show grid or not
    /// </summary>
    public bool showGrid = true;


    /// <summary>
    /// how fast for the gird moving when up key ctrol
    /// </summary>
    float shift_amount = 1;
    List<MP_ScreenGrid> screenGrids = new List<MP_ScreenGrid>();


    /// <summary>
    /// adjust blend gamma 
    /// </summary>
    float edge_gamma = 0.31f;

    MP_ScreenGrid.MOVE_WAY move_way = MP_ScreenGrid.MOVE_WAY.SINGLE;


    /// <summary>
    /// global_array_foucs_col / row using for global index when change grid ctrol point
    /// </summary>
    int global_array_foucs_col = 0;
    int global_array_foucs_row = 0;

    /// <summary>
    /// max cols rows record size of screens , depends on screens count
    /// </summary>
    int max_cols = 0;
    int max_rows = 0;


    /// <summary>
    /// last active screen id
    /// </summary>
    int last_screen_id = -1;

    int screen_count = 0;


    /// <summary>
    /// screen rows and cols
    /// </summary>
    int screen_count_col = 1; 
    int screen_count_row = 1;

    int screen_width = 1024;
    int screen_height = 768;


    /// <summary>
    /// update screen_width by resolution
    /// </summary>
    void set_screen_w_h()
    {
        if (screen_reslotion == ScreenResolution.S_1024X768)
        {
            screen_width = 1024;
            screen_height = 768;
        }
        else if (screen_reslotion == ScreenResolution.S_800X600)
        {
            screen_width = 800;
            screen_height = 600;
        }
        else if (screen_reslotion == ScreenResolution.S_1280X720)
        {
            screen_width = 1280;
            screen_height = 720;
        }
        else if (screen_reslotion == ScreenResolution.S_1280X800)
        {
            screen_width = 1280;
            screen_height = 800;
        }
        else if (screen_reslotion == ScreenResolution.S_1920X1080)
        {
            screen_width = 1920;
            screen_height = 1080;
        }
        else if (screen_reslotion == ScreenResolution.S_1920X1200)
        {
            screen_width = 1920;
            screen_height = 1200;
        }

    }

    /// <summary>
    /// previous camera
    /// </summary>
    Camera old_Cam = null;

    /// <summary>
    /// active monitors and create texture
    /// </summary>
    void Awake()
    {

        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();

        }

        instance = this;

        set_screen_w_h();
        if (layout == ScreenLayout.L_1X1)
        {
            screen_count = 1;
            screen_count_col = 1;
            screen_count_row = 1;
            targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        }
        else if (layout >= ScreenLayout.L_2X1 && layout <= ScreenLayout.L_8X1)
        {
            screen_count = ((int)layout - (int)ScreenLayout.L_2X1 + 2);
            screen_count_col = screen_count;
            screen_count_row = 1;
            targetTexture = new RenderTexture(screen_width * screen_count, screen_height, 24);
        }
        else if (layout == ScreenLayout.L_2X2)
        {
            screen_count = 4;
            screen_count_col = 2;
            screen_count_row = 2;
            targetTexture = new RenderTexture(screen_width * screen_count_col, screen_height* screen_count_row, 24);
        }
        else if (layout == ScreenLayout.L_2X3)
        {
            screen_count = 6;
            screen_count_col = 2;
            screen_count_row = 3;
            targetTexture = new RenderTexture(screen_width * screen_count_col, screen_height * screen_count_row, 24);
        }
        else if (layout == ScreenLayout.L_3X2)
        {
            screen_count = 6;
            screen_count_col = 3;
            screen_count_row = 2;
            targetTexture = new RenderTexture(screen_width * screen_count_col, screen_height * screen_count_row, 24);
        }
        else if (layout == ScreenLayout.L_4X2)
        {
            screen_count = 8;
            screen_count_col = 4;
            screen_count_row = 2;
            targetTexture = new RenderTexture(screen_width * screen_count_col, screen_height * screen_count_row, 24);
        }
        else if (layout == ScreenLayout.L_2X4)
        {
            screen_count = 8;
            screen_count_col = 2;
            screen_count_row = 4;
            targetTexture = new RenderTexture(screen_width * screen_count_col, screen_height * screen_count_row, 24);
        } 

        targetTexture.antiAliasing = 2;
        targetTexture.format = RenderTextureFormat.ARGB32;
        targetTexture.Create();


        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {

            if (cam.name != "MP_MappingCamera")
            {
                cam.targetTexture = targetTexture;
                cam.tag = "Untagged"; //disable camera main tag,  Mapping camera will take over the last show
                old_Cam = cam;
                break;
            }
        }

        blend_count = (BlendAreaCount)PlayerPrefs.GetInt("blend_count", 4);
    }

    void start_1X1()
    {
        max_cols = COLUMNS + 1;
        max_rows = ROWS + 1;

        GameObject screen = Instantiate(screen_pre) as GameObject;

        screen.transform.position = new Vector3(40, 0, -10f);
        Transform screen_grid = screen.transform.Find("MP_ScreenGrid");

        MP_ScreenGrid sg = screen_grid.GetComponentInChildren<MP_ScreenGrid>();
        sg.set_mesh_uv(new Vector4(0, 0, 1f, 1f), targetTexture);
        sg.screen_width = Screen.width;
        sg.screen_height = Screen.height;
        sg.SetGridCount(COLUMNS, ROWS);
        sg.showGrid = showGrid;
        sg.isFocus = true;
        sg.blend_count = (int)blend_count +1;
        screenGrids.Add(sg);
    }

    void start_nX1()
    {

        int iblend_count = (int)blend_count ;
        //print(iblend_count);
        // how many screen 

        max_cols = screen_count * (COLUMNS + 1);
        max_rows = ROWS + 1;

        // b_uv_step is a all width include normal and blend area
        // b_uv_step_u is overlap blend width in texture 
        float b_uv_step = COLUMNS * 1.0f / (COLUMNS * screen_count - iblend_count * (screen_count - 1));
        float b_uv_step_u = iblend_count * 1.0f / COLUMNS * b_uv_step;

        for (int i = 0; i < screen_count; i++)
        {
            GameObject screen = Instantiate(screen_pre) as GameObject;
            screen.transform.position = transform.position + new Vector3((i + 1) * 20 + 20, 0, -10f);
            Transform screen_grid = screen.transform.Find("MP_ScreenGrid");
            float u_start = (b_uv_step - b_uv_step_u) * i;
            Vector4 uv_rect = new Vector4(u_start, 0f, b_uv_step, 1);
            MP_ScreenGrid sg = screen_grid.GetComponent<MP_ScreenGrid>();
            sg.set_mesh_uv(uv_rect, targetTexture);
            sg.screen_id = i;
            sg.SetGridCount(COLUMNS, ROWS);

            // in editor mode only use first monitor width
#if UNITY_EDITOR
            sg.screen_width = Screen.width;
            sg.screen_height = Screen.height;
#else
                 
            
                if (sg.screen_id < Display.displays.Length)
                {
                    sg.screen_width = Display.displays[sg.screen_id].systemWidth;
                    sg.screen_height = Display.displays[sg.screen_id].systemHeight;
                }
                else
                {
                    sg.screen_width = screen_width;
                    sg.screen_height = screen_height;
                }

#endif

            sg.GirdColor = Color.blue;
            sg.showGrid = showGrid;
            sg.isFocus = false;
            sg.blend_count = (int)blend_count + 1;
            if (i == 0) sg.isFocus = true;
            screenGrids.Add(sg);
        }
    }

    void start_nXn()
    {
        int iblend_count = (int)blend_count ;


        max_cols = screen_count_col * (COLUMNS + 1);
        max_rows = screen_count_row * (ROWS + 1);

        // b_uv_step is a all width include normal and blend area
        // b_uv_step_u is overlap blend width in texture 
        float b_u_step = COLUMNS * 1.0f / (COLUMNS * screen_count_col - iblend_count * (screen_count_col - 1));
        float b_u_step_u = iblend_count * 1.0f / COLUMNS * b_u_step;

        float b_v_step = ROWS * 1.0f / (ROWS * screen_count_row - iblend_count * (screen_count_row - 1));
        float b_v_step_v = iblend_count * 1.0f / ROWS * b_v_step;

        for (int i = 0; i < screen_count_col; i++)
            for (int j = 0; j < screen_count_row; j++)
            {
                GameObject screen = Instantiate(screen_pre) as GameObject;
                screen.transform.position = transform.position + new Vector3((i + 1) * 20 + 20, (j + 1) * -20 + 20, -10f);
                Transform screen_grid = screen.transform.Find("MP_ScreenGrid");
                float u_start = (b_u_step - b_u_step_u) * i;
                float v_start = (b_v_step - b_v_step_v) * (screen_count_row-1-j);
                Vector4 uv_rect = new Vector4(u_start, v_start, b_u_step, b_v_step);
                MP_ScreenGrid sg = screen_grid.GetComponent<MP_ScreenGrid>();
                sg.set_mesh_uv(uv_rect, targetTexture);
                sg.screen_id = i+j* screen_count_col;
                sg.SetGridCount(COLUMNS, ROWS);

                // in editor mode only use first monitor width
#if UNITY_EDITOR
                sg.screen_width = Screen.width;
                sg.screen_height = Screen.height;
#else
            
                if (sg.screen_id < Display.displays.Length)
                {
                    sg.screen_width = Display.displays[sg.screen_id].systemWidth;
                    sg.screen_height = Display.displays[sg.screen_id].systemHeight;
                }
                else
                {
                    sg.screen_width = screen_width;
                    sg.screen_height = screen_height;
                }

#endif

                sg.GirdColor = Color.blue;
                sg.showGrid = showGrid;
                sg.isFocus = false;
                sg.blend_count = (int)blend_count + 1;
                if (i == 0) sg.isFocus = true;
                screenGrids.Add(sg);
            }
    }
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        global_array_foucs_col = COLUMNS / 2; //default col is half of screen
        global_array_foucs_row = ROWS / 2;
         

        if (layout == ScreenLayout.L_1X1)
        {
            start_1X1();
        }
        else if (layout >= ScreenLayout.L_2X2 && layout <= ScreenLayout.L_4X2)
        {
            start_nXn();
        }
        else if (layout >= ScreenLayout.L_2X1 && layout <= ScreenLayout.L_8X1)
        {
            start_nX1();
        }        
    }



    /// <summary>
    /// active a gird and update it's color
    /// </summary>
    /// <param name="dst_screen_id"></param>
    void setActiveScreen(int dst_screen_id)
    {

        foreach (MP_ScreenGrid sg in screenGrids)
        {
            if (sg.screen_id == dst_screen_id)
            {
                sg.isFocus = true;
                sg.GirdColor = Color.green;
            }
            else
            {
                sg.isFocus = false;
                sg.GirdColor = Color.blue;
            }
            sg.UpdateLines();
        }

        last_screen_id = dst_screen_id;
    }

    /// <summary>
    /// get active screen
    /// </summary>
    /// <returns></returns>
    MP_ScreenGrid getActiveScreen()
    {
        foreach (MP_ScreenGrid sg in screenGrids)
        {
            if (sg.isFocus)
            {
                return sg;
            }
        }
        Debug.LogError("can not find any focus screen!");
        return null;
    }


    /// <summary>
    /// make sure row and col in safe range
    /// </summary>
    void check_foucs_col_row_border()
    {
        if (global_array_foucs_row < 0)
            global_array_foucs_row = 0;
        if (global_array_foucs_col < 0)
            global_array_foucs_col = 0;
        if (global_array_foucs_row > max_rows - 1)
            global_array_foucs_row = max_rows - 1;
        if (global_array_foucs_col > max_cols - 1)
            global_array_foucs_col = max_cols - 1;
    }

    /// <summary>
    /// move ctrol point
    /// </summary>
    /// <param name="move_dir"></param>
    void move_ctrl_pt(int[] move_dir)
    {

        global_array_foucs_col += move_dir[0];
        global_array_foucs_row += move_dir[1];

        check_foucs_col_row_border();
        int dst_screen_id_x = global_array_foucs_col / (COLUMNS + 1);
        int dst_screen_id_y = global_array_foucs_row / (ROWS + 1);

        int dst_screen_id = dst_screen_id_x + (screen_count_row- dst_screen_id_y-1) * screen_count_col;

        int col = global_array_foucs_col % (COLUMNS + 1);
        int row = global_array_foucs_row % (ROWS + 1);
        //if move gird across screen , change active screen
        if (dst_screen_id != last_screen_id && move_way != MP_ScreenGrid.MOVE_WAY.FOUR_CONNER)
        {
            setActiveScreen(dst_screen_id);
        }

        if (move_way != MP_ScreenGrid.MOVE_WAY.FOUR_CONNER)
        {
            getActiveScreen().set_ctrl_pt(col, row);
        }
        else
        {
            // if only four_conner just move conner only
            bool dir = false;
            if ((move_dir[0] + move_dir[1]) > 0) dir = true;
            getActiveScreen().set_conner_pt(dir);
        }

    }


    bool isShowGamaLine = false;

    /// <summary>
    /// update gamma 
    /// </summary>
    void update_edge_gamma()
    {
        getActiveScreen().update_gamma(edge_gamma, isShowGamaLine);
        showInfor("now gamma is:" + edge_gamma);
    }

    public bool showColorPanel = true;
    public bool showHelp = true;
    public GUIStyle infor_style;
    public GUIStyle help_style;
    float last_msg_time = 0;
    string infor = "";

    /// <summary>
    /// show message 
    /// </summary>
    /// <param name="msg"></param>
    public void showInfor(string msg)
    {
        last_msg_time = Time.time;
        infor = msg;
    }


    /// <summary>
    /// color panel for RGB and gamma
    /// </summary>

    Vector3 RGB_base = Vector3.zero;


    /// <summary>
    /// use for Black Level and Brightness Compensation
    /// https://resolume.com/manual/start?id=en/r4/output
    /// </summary>
    Vector3 RGB_Compensation_base = Vector3.zero; 


    void help_gui_do_win(int windowID)
    {
        string helpStr = "";
        helpStr += "F1: show help or not \n";
        helpStr += "F2: show color panel or not \n";
        helpStr += "H: hide grid or not\n";
        helpStr += "W/S/A/D : choose control point \n";
        helpStr += "Up/Left/Down/Right : move control point \n (Press Shift to speed up and Alt to slow)\n";
        helpStr += "M: change grid move way \n";
        helpStr += "R/G/B: change screen color, Up/Down change color value \n";
        helpStr += "num 1-num4: open blend area(left/right/top/bottom)\n";
        helpStr += "-,=: adjust blend area gamma. \n";
        helpStr += "Mouse click: choose screen \n";
        helpStr += "Tab: switch screen \n";
        helpStr += "T: testing screen \n";
        helpStr += "C: Change blend area count [1,8], need restart software. \n";
        helpStr += "F5: save all data\n";
        helpStr += "F8: load default grid \n";
        helpStr += "ESC: exit\n";

        Rect rect = new Rect(screen_width / 8, 50, 800, 900);
        GUI.Box(new Rect(0, 0, screen_width, screen_width), "");
        GUI.Label(rect, helpStr, help_style);

    }

    void win_color(int w_id)
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        int y_pos = 20;
        Vector3 RGB_base_old = RGB_base;
        GUI.Label(new Rect(5, y_pos, 20, 30), "R");
        RGB_base.x = GUI.HorizontalSlider(new Rect(25, y_pos + 5, 500, 30), RGB_base.x, -1f, 1f);

        y_pos += 30;
        GUI.Label(new Rect(5, y_pos, 20, 30), "G");
        RGB_base.y = GUI.HorizontalSlider(new Rect(25, y_pos + 5, 500, 30), RGB_base.y, -1f, 1f);
        y_pos += 30;
        GUI.Label(new Rect(5, y_pos, 20, 30), "B");
        RGB_base.z = GUI.HorizontalSlider(new Rect(25, y_pos + 5, 500, 30), RGB_base.z, -1f, 1f);
        y_pos += 30;
        if (IsVector3Changed(RGB_base, RGB_base_old))
        {
            getActiveScreen().set_screen_color_directly(RGB_base);
        }

        Vector3 RGB_Compensation_base_old = RGB_Compensation_base;
        GUI.Label(new Rect(5, y_pos, 40, 30), "C_R");
        RGB_Compensation_base.x = GUI.HorizontalSlider(new Rect(45, y_pos + 5, 480, 30), RGB_Compensation_base.x, -0.5f, 0.5f);
        y_pos += 30;
        GUI.Label(new Rect(5, y_pos, 40, 30), "C_G");
        RGB_Compensation_base.y = GUI.HorizontalSlider(new Rect(45, y_pos + 5, 480, 30), RGB_Compensation_base.y, -0.5f, 0.5f);
        y_pos += 30;
        GUI.Label(new Rect(5, y_pos, 40, 30), "C_B");
        RGB_Compensation_base.z = GUI.HorizontalSlider(new Rect(45, y_pos + 5, 480, 30), RGB_Compensation_base.z, -0.5f, 0.5f);
        y_pos += 30;
        if (IsVector3Changed(RGB_Compensation_base, RGB_Compensation_base_old))
        {
            getActiveScreen().set_compsention_color(RGB_Compensation_base);
        }



        GUI.Label(new Rect(5, y_pos, 60, 30), "Gamma");
        float old_edge_gamma = edge_gamma;
        edge_gamma = GUI.HorizontalSlider(new Rect(65, y_pos + 5, 470, 30), edge_gamma, 0f, 1f);
        if (Mathf.Abs(edge_gamma - old_edge_gamma) > 0.00001f)
        {
            isShowGamaLine = true;
            update_edge_gamma();
        }
        y_pos += 30;

        if (GUI.Button(new Rect(100, y_pos, 100, 30), "Reset All"))
        {
            RGB_base = Vector3.zero;
            getActiveScreen().set_screen_color_directly(RGB_base);
            edge_gamma = 0.31f;
            update_edge_gamma();
            RGB_Compensation_base = Vector3.zero;
            getActiveScreen().set_compsention_color(RGB_Compensation_base);

        }

    }
    Rect windowRect = new Rect(20, 20, 550, 280);

    void OnGUI()
    {
        if ((Time.time - last_msg_time) < 3f)
        {
            GUI.Label(new Rect(40, 100, 1000, 100), infor, infor_style);
        }

        if (showHelp)
        {
            help_gui_do_win(0);
        }


        if (showColorPanel)
        {
            windowRect = GUI.Window(0, windowRect, win_color, "RGB & Gamma");
        }
 
    }

 
    /// <summary>
    /// is the vector3 value changed ?
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    bool IsVector3Changed(Vector3 v1, Vector3 v2)
    {
        Vector3 diff = v1 - v2;
        if (diff.sqrMagnitude > 0.00001f) return true; 
        return false;
    }

    /// <summary>
    /// get screen id by mouse , now support 1*n .  
    /// </summary>
    /// <returns></returns>
    int getScreenIDbyMousePostion( )
    {
   
        float start_x = 0;
        int sc_id = 0;
        if (screen_count <= Display.displays.Length)
        {
            for (int i = 0; i < screen_count; i++)
            {
                if (Input.mousePosition.x > start_x)
                {
                    sc_id = i;                    
                }
                start_x += Display.displays[i].systemWidth;
            }
        }
        return sc_id;
    }
    /// <summary>
    /// when mouse click , find near point and active screen
    /// </summary>
    void setActiveScreenByMouse()
    {
       
        

        Vector2 screen_mouse_start_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (!showGrid) return;
        int dst_screen_id = 0;
        if (layout == ScreenLayout.L_1X1)
        {
            setActiveScreen(dst_screen_id);
        }
        else if (layout == ScreenLayout.L_2X2)
        {

        }
        else if (layout == ScreenLayout.L_2X1 || (layout == ScreenLayout.L_3X1) || (layout == ScreenLayout.L_4X1))
        {
            // in editor  can not get multi-screen mouse position, so only switch screen,not pick point
#if UNITY_EDITOR
            //dst_screen_id = last_screen_id + 1;
            //dst_screen_id = dst_screen_id % screen_count;
            //setActiveScreen(dst_screen_id);
#else
                 
            Vector3 relative_mouse = Display.RelativeMouseAt(Input.mousePosition);
            dst_screen_id = getScreenIDbyMousePostion();
            setActiveScreen(dst_screen_id);
            screen_mouse_start_pos = new Vector2(relative_mouse.x, relative_mouse.y);

#endif

        }

        int near_col = 0;
        int near_row = 0;
        getActiveScreen().getNearest_Col_row(screen_mouse_start_pos.x, screen_mouse_start_pos.y, out near_col, out near_row);


        global_array_foucs_col = dst_screen_id * (COLUMNS + 1) + near_col;
        global_array_foucs_row = near_row;
        check_foucs_col_row_border();

        int col = global_array_foucs_col % (COLUMNS + 1);
        int row = global_array_foucs_row % (ROWS + 1);
        getActiveScreen().set_ctrl_pt(col, row);
    }

    /// <summary>
    /// is01SameValue check same return 0 or 1 , for math calculate
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    int is01SameValue(CtrolObject c1, CtrolObject c2)
    {
        if (c1 == c2) return 1;
        return 0;
    }
    /// <summary>
    /// up / left key handle
    /// change value by CtrolObject type
    /// </summary>
    void holdDirKey()
    {
        if (Input.GetKey(KeyCode.R))
        {
            ctrol_object = CtrolObject.C_COLOR_R;
            showInfor("change color red now");
        }
        else if (Input.GetKey(KeyCode.G))
        {
            ctrol_object = CtrolObject.C_COLOR_G;
            showInfor("change color green now");
        }
        else if (Input.GetKey(KeyCode.B))
        {
            ctrol_object = CtrolObject.C_COLOR_B;
            showInfor("change color blue now");
        }
        else if (Input.GetKey(KeyCode.M))
        {
            ctrol_object = CtrolObject.C_GRID;
            //showInfor("change grid now");
        }

        if (ctrol_object == CtrolObject.C_GRID && showGrid)
        {
            if (Input.GetKeyUp(KeyCode.D))
            {
                int[] move_dir = new int[] { 1, 0 };
                move_ctrl_pt(move_dir);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                int[] move_dir = new int[] { -1, 0 };
                move_ctrl_pt(move_dir);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                int[] move_dir = new int[] { 0, 1 };
                move_ctrl_pt(move_dir);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                int[] move_dir = new int[] { 0, -1 };
                move_ctrl_pt(move_dir);
            }

            Vector2 move_shift = Vector2.zero;
            bool findMove = false;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                move_shift = new Vector2(0, 0.0005f * shift_amount);
                findMove = true;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                move_shift = new Vector2(0, -0.0005f * shift_amount);
                findMove = true;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move_shift = new Vector2(-0.0005f * shift_amount, 0);
                findMove = true;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                move_shift = new Vector2(0.0005f * shift_amount, 0);
                findMove = true;
            }
            if (findMove) getActiveScreen().set_move_gird(move_shift);
        }
        else if (ctrol_object == CtrolObject.C_COLOR_R || ctrol_object == CtrolObject.C_COLOR_G || ctrol_object == CtrolObject.C_COLOR_B)
        {
            Vector3 color_shift = Vector3.zero;
            bool findMove = false;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                float mount = -0.001f * shift_amount;
                color_shift = new Vector3(is01SameValue(ctrol_object, CtrolObject.C_COLOR_R) ,
                    is01SameValue(ctrol_object, CtrolObject.C_COLOR_G) ,
                    is01SameValue(ctrol_object, CtrolObject.C_COLOR_B) ) * mount;
                findMove = true;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                float mount = 0.001f * shift_amount;
                color_shift = new Vector3(is01SameValue(ctrol_object, CtrolObject.C_COLOR_R),
                    is01SameValue(ctrol_object, CtrolObject.C_COLOR_G),
                    is01SameValue(ctrol_object, CtrolObject.C_COLOR_B)) * mount;
                findMove = true;
            }
            if (findMove)
            {
               
                getActiveScreen().set_screen_color(color_shift);
                RGB_base = getActiveScreen().get_screen_color();
                RGB_Compensation_base = getActiveScreen().get_compsention_color();
            }
        }

    }


    /// <summary>
    /// clear old camera color for test color
    /// </summary>
    int old_cam_index = 0;
    void set_old_cam_color()
    {
        Color[] test_colors = { Color.yellow, Color.white, Color.black, Color.red, Color.magenta };
        old_cam_index++;
        old_cam_index = old_cam_index % test_colors.Length;
        if (old_Cam)
        {
            for (int i = 0; i < old_Cam.transform.childCount; i++)
            {
                Transform cld = old_Cam.transform.GetChild(i);
                Destroy(cld.gameObject);
                
            }
            old_Cam.backgroundColor = test_colors[old_cam_index];
        }

    }

    /// <summary>
    /// change screen or open color panel
    /// </summary>
    void update_color_gamma()
    {
        RGB_base = getActiveScreen().get_screen_color();
        RGB_Compensation_base = getActiveScreen().get_compsention_color();
        float gamma = getActiveScreen().get_gamma();
        if (gamma > 0) edge_gamma = gamma;
    }
    void reset_confirm_count()
    {
        confirm_count = 0;
    }
    int confirm_count = 0;
    int exit_count = 0;
    int temp_blend_count = -1;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            shift_amount = 5.0f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            shift_amount = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            shift_amount = 0.5f;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            shift_amount = 1.0f;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            exit_count++;
            if (exit_count >= 2)
            {
                Application.Quit();
            }
            else
            {
                showInfor("will exit.did you Press F5 Save? (exit Press ESC again )");
            }

        }

        if (Input.GetKeyUp(KeyCode.F1))
        {
            showHelp = !showHelp;
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            showColorPanel = !showColorPanel;
            if (showColorPanel) update_color_gamma();
            else
            {
                isShowGamaLine = false;
                getActiveScreen().update_gamma(edge_gamma, isShowGamaLine);
            }
        }

       

        if (Input.GetKeyUp(KeyCode.H))
        {
            showGrid = !showGrid;
            foreach (MP_ScreenGrid sg in screenGrids)
            {
                sg.ShowOrHideGrid(showGrid);
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            getActiveScreen().change_edge(0);
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            getActiveScreen().change_edge(1);
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            getActiveScreen().change_edge(2);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            getActiveScreen().change_edge(3);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            int dst_screen_id = last_screen_id + 1;
            dst_screen_id = dst_screen_id % screen_count;
            setActiveScreen(dst_screen_id);
            update_color_gamma();
            showInfor("Screen :" + dst_screen_id + " is acitved") ;
        }

        
        if (Input.GetKey(KeyCode.Equals))
        {
            edge_gamma += 0.002f* shift_amount;
            if (edge_gamma > 1) edge_gamma = 1;
            isShowGamaLine = true;
            update_edge_gamma();
          
        }
        if (Input.GetKey(KeyCode.Minus))
        {
            edge_gamma -= 0.002f* shift_amount;
            if (edge_gamma < 0) edge_gamma = 0;
            isShowGamaLine = true;
            update_edge_gamma();
            
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            set_old_cam_color();

        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            if (temp_blend_count < 0) temp_blend_count = (int)blend_count;
            temp_blend_count++;
            if (temp_blend_count > 7) temp_blend_count = 1;
            PlayerPrefs.SetInt("blend_count", temp_blend_count);
            showInfor("Set blend count to:" + temp_blend_count + ". please restart software to acitve it");

        }



        if (Input.GetKeyUp(KeyCode.F5))
        {

            foreach (MP_ScreenGrid sg in screenGrids)
            {
                sg.Save();
            }
          
            showInfor("Save OK.");
        }

        holdDirKey();
        if (!showGrid) return; //rest key all need gird show

        if (Input.GetKeyUp(KeyCode.M))
        {
            move_way++;
            if ((int)(move_way) >= ((int)MP_ScreenGrid.MOVE_WAY.SINGLE_SMOOTH + 1))
                move_way = MP_ScreenGrid.MOVE_WAY.SINGLE;

            foreach (MP_ScreenGrid sg in screenGrids)
            {
                sg.set_move_way(move_way);
            }
            showInfor("now move grid by:" + move_way.ToString());
        }



        if (Input.GetMouseButtonUp(0))
        {
            setActiveScreenByMouse();
        }


        if (Input.GetKeyUp(KeyCode.F8))
        {
            confirm_count++;
            if (confirm_count >= 2)
            {
                foreach (MP_ScreenGrid sg in screenGrids)
                {
                    sg.LoadDefault();
                }
                showInfor("load default postion .");
            }
            else
            {
                Invoke("reset_confirm_count", 3f);
                showInfor("will load default postion.? (Press F8 again )");

            }
        }
    }
}