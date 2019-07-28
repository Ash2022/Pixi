using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    int m_curr_level;
    int m_score;

    ManagerView.BubbleColors m_last_popped_bubble_type = ManagerView.BubbleColors.Black;
    int m_bubbles_in_combo = 0;
    int m_num_turns_left = 0;

    /*
    float m_health = 1000;
    float m_decrease_rate = 50;
    */
    bool m_game_active = false;
    bool m_special_power_up_engaged = false;

    bool m_bubbles_can_click = true;

    const float INITIAL_DECREASE_RATE = 50;
    const float TOTAL_FULL_TIME = 20f;
    const int BASIC_BUBBLE_SCORE = 10;

    public bool Game_active { get => m_game_active; set => m_game_active = value; }
    public int Score { get => m_score; set => m_score = value; }
    public bool Bubbles_can_click { get => m_bubbles_can_click; set => m_bubbles_can_click = value; }
    public bool During_drag { get => during_drag; set => during_drag = value; }
    bool m_bubble_popped_in_click = false;

    private bool during_drag = false;

    private void Awake()
    {
        Instance = this;
    }

    private void UpdateTurnsDisplay()
    {
        ManagerView.Instance.Turns_text.text = "TURNS: " + m_num_turns_left;
    }

    public void TurnEnded()
    {
        Debug.Log("Mouse UP");

        if (m_bubble_popped_in_click)
            ScoreAndResetCombo();

        During_drag = false;
        m_num_turns_left--;
        UpdateTurnsDisplay();


        if (m_num_turns_left == 0)
        {
            m_game_active = false;
            ManagerView.Instance.GameOver();
        }



    }

    private void ScoreAndResetCombo()
    {
        //score last combo if any
        int add_to_score = 0;


        for (int i = 1; i <= m_bubbles_in_combo + 1; i++)
        {
            add_to_score += i * BASIC_BUBBLE_SCORE;
        }

        UpdateScore(add_to_score, " " + (m_bubbles_in_combo + 1) + " in combo");


        During_drag = false;
        m_bubbles_in_combo = 0;
    }

    public void PowerUpUsed()
    {
        //add a turn - as it counts the mouse up - but there was no turn
        Bubbles_can_click = true;
        m_num_turns_left++;
        UpdateTurnsDisplay();
    }

    public void Button_StartGameClicked()
    {
        m_curr_level = 0;
        ResetLevel();
        ManagerView.Instance.StartGame();
        //StartCoroutine(GameTimer());
        StartCoroutine(ManagerView.Instance.GenerateBalls(35, GetCurrLevelRange(), GetCurrNumColors(), 0.01f));
    }

    private ModelManager.Level GetCurrLevel()
    {
        return ModelManager.Instance.Levels[m_curr_level];
    }

    public Vector2 GetCurrLevelRange()
    {
        return ModelManager.Instance.Levels[m_curr_level].GetSizeRange();
    }

    public int GetCurrNumColors()
    {
        return ModelManager.Instance.Levels[m_curr_level].Num_colors;
    }

    public void Button_NextLevelClicked()
    {

    }

    private void ResetLevel()
    {
        m_last_popped_bubble_type = ManagerView.BubbleColors.Black;
        m_bubbles_in_combo = 0;
        m_game_active = true;
        m_num_turns_left = ModelManager.Instance.Levels[m_curr_level].Num_turns;
        UpdateScore(0, "Reset", true);
        UpdateTurnsDisplay();
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButton(0) == false)
        {
            if (During_drag)
                TurnEnded();

            During_drag = false;

        }



#elif UNITY_ANDROID
       
        if (Input.touchCount == 0)
        {
            if (m_prev_frame_mouse_down)
                TurnEnded();
            else
                m_prev_frame_mouse_down = false;
        }
        else
            m_prev_frame_mouse_down = true;

#endif



    }



    public void CreatePowerUps(Vector2 pos)
    {
        if (UnityEngine.Random.Range(0, 1f) > 0.9f)
        {
            float new_range = UnityEngine.Random.Range(0, 1f);
            //Debug.Log("new_range " + new_range);

            if (new_range < 0.5f)
            {
                ManagerView.Instance.GeneratePowerUp(1, pos);
            }/*
            else if (new_range >= 0.3f && new_range < 0.9f)
            {
                ManagerView.Instance.GeneratePowerUp(2, pos);
            }*/
            else if (new_range >= 0.5)
            {
                ManagerView.Instance.GeneratePowerUp(3, pos);
            }
        }
    }


    public bool BubblePopped(ManagerView.BubbleColors bubbleColors, Vector2 pos, BallView ballView)
    {
        if (Game_active == false)
            return false;

        if (m_special_power_up_engaged)
        {
            List<string> vs = new List<string>();

            ManagerView.Instance.AddMyTouchingRecursive(ballView, vs);

            for (int i = 0; i < vs.Count; i++)
                ManagerView.Instance.KillObject(vs[i]);

            //score for chain kill vs.count
            UpdateScore(BASIC_BUBBLE_SCORE * vs.Count * 2, " Chain kill " + vs.Count + " in chain");

            m_special_power_up_engaged = false;
            return false;
        }

        m_bubble_popped_in_click = false;

        if (bubbleColors == m_last_popped_bubble_type)
            m_bubbles_in_combo++;
        else
        {
            if(m_bubbles_in_combo<=0)
            {
                m_bubble_popped_in_click = true;
            }
            else
            {
                //color swap in combo
                
                ScoreAndResetCombo();
                m_last_popped_bubble_type = ManagerView.BubbleColors.Black;
                return false;
            }

        }
       
        
            m_last_popped_bubble_type = bubbleColors;

            ManagerView.Instance.GenerateRandomBubble(true, GameController.Instance.GetCurrLevelRange(), GameController.Instance.GetCurrNumColors());

            CreatePowerUps(pos);

      

        //ScoreAndResetCombo();   

        Debug.Log(m_bubbles_in_combo);

                return true;

    }

    public void HealthUp()
    {
        //m_health += 20;
        PowerUpUsed();
    }

    public void KillAll()
    {
        m_special_power_up_engaged = true;
        PowerUpUsed();
    }

    public void Bomb(List<string> colliders_in_range)
    {
        int total_killed = 0;

        for (int i = 0; i < colliders_in_range.Count; i++)
        {
            //might be that the same bubble is inside many colliders
            if (ManagerView.Instance.InList(colliders_in_range[i]))
            {
                total_killed += ManagerView.Instance.KillObject(colliders_in_range[i]);
            }
        }

        //score total killed in bomb
        UpdateScore(BASIC_BUBBLE_SCORE * total_killed + total_killed, " Bomb kill " + total_killed);

        PowerUpUsed();
    }

    /*
    IEnumerator GameTimer()
    {
        float start_time = Time.time;
        float total_time = m_health / m_decrease_rate;
        float time_left = total_time - (Time.time - start_time);

        //Debug.Log(total_time);
        //Debug.Log(time_left);

        while (time_left > 0)
        {

            int time_passed = Convert.ToInt32((Time.time - start_time) / 5);

            m_decrease_rate = INITIAL_DECREASE_RATE + time_passed * 5;

            m_health -= Time.deltaTime * m_decrease_rate;

            m_health = Math.Min(m_health, TOTAL_FULL_TIME * INITIAL_DECREASE_RATE);

            time_left = m_health / m_decrease_rate;

            ManagerView.Instance.Bar_fill.fillAmount = time_left / TOTAL_FULL_TIME;

            yield return null;
        }

        m_game_active = false;
        TimerExpired();
    }
    */


    private void UpdateScore(int added_to_score, string info, bool reset = false)
    {
        if (reset)
            m_score = 0;
        else
            m_score += added_to_score;

        ManagerView.Instance.Score_info_text.text = "Added: " + added_to_score + info;

        ManagerView.Instance.Score_text.text = "SCORE: " + m_score;

    }/*
    private void TimerExpired()
    {
        ManagerView.Instance.TimerExpired();
    }*/

}
