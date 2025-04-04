using UnityEngine;

public partial class MP_MultiScreen_X : MonoBehaviour
{
    [SerializeField] [Range(-1f, 1f)] float overlap = 0f;
    void OverlapTest()
    {
        if (layout == ScreenLayout.L_1X1)
        {
            // start_1X1();
            var screen_grid = this.screens[0].transform.Find("MP_ScreenGrid");
            var sg = screen_grid.GetComponentInChildren<MP_ScreenGrid>();
            sg.update_mesh_uv(new Vector4(this.overlap, 0, 1f, 1f));
        }
        else if (layout >= ScreenLayout.L_2X2 && layout <= ScreenLayout.L_4X2)
        {
            // start_nXn();

            int iblend_count = (int)blend_count;

            // b_uv_step is a all width include normal and blend area
            // b_uv_step_u is overlap blend width in texture 
            float b_u_step = COLUMNS * 1.0f / (COLUMNS * screen_count_col - iblend_count * (screen_count_col - 1));
            float b_u_step_u = iblend_count * 1.0f / COLUMNS * b_u_step;

            float b_v_step = ROWS * 1.0f / (ROWS * screen_count_row - iblend_count * (screen_count_row - 1));
            float b_v_step_v = iblend_count * 1.0f / ROWS * b_v_step;

            for (int i = 0; i < screen_count_col; i++)
                for (int j = 0; j < screen_count_row; j++)
                {
                    var screen = this.screens[i * screen_count_row + j];
                    Transform screen_grid = screen.transform.Find("MP_ScreenGrid");
                    float u_start = (b_u_step - b_u_step_u) * i + this.overlap
                        ;
                    float v_start = (b_v_step - b_v_step_v) * (screen_count_row - 1 - j);
                    Vector4 uv_rect = new Vector4(u_start, v_start, b_u_step, b_v_step);
                    MP_ScreenGrid sg = screen_grid.GetComponent<MP_ScreenGrid>();
                    sg.update_mesh_uv(uv_rect);
                }
        }
        else if (layout >= ScreenLayout.L_2X1 && layout <= ScreenLayout.L_8X1)
        {
            // start_nX1();

            int iblend_count = (int)this.blend_count;
            //print(iblend_count);

            // how many screen 

            max_cols = screen_count * (COLUMNS + 1);
            max_rows = ROWS + 1;

            // b_uv_step is a all width include normal and blend area
            // b_uv_step_u is overlap blend width in texture 
            float b_uv_step = COLUMNS * 1.0f / (COLUMNS * screen_count - iblend_count * (screen_count - 1));
            float b_uv_step_u = iblend_count * 1.0f / COLUMNS * b_uv_step;

            for (var i = 0; i < this.screens.Count; ++i)
            {
                var screen = this.screens[i];
                Transform screen_grid = screen.transform.Find("MP_ScreenGrid");
                float u_start = (b_uv_step - b_uv_step_u) * i + this.overlap
                    ;
                Vector4 uv_rect = new Vector4(u_start, 0f, b_uv_step, 1);
                MP_ScreenGrid sg = screen_grid.GetComponent<MP_ScreenGrid>();
                sg.update_mesh_uv(uv_rect);
            }
        }
    }
}
