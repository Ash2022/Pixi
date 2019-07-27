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

    float m_health = 1000;
    float m_decrease_rate = 50;

    bool m_game_active = false;
    bool m_special_power_up = false;
    bool m_mouse_went_up = false;

    const float INITIAL_DECREASE_RATE = 50;

    const float TOTAL_FULL_TIME = 20f;

    public bool Game_active { get => m_game_active; set => m_game_active = value; }
    public int Score { get => m_score; set => m_score = value; }

    private void Awake()
    {
        Instance = this;
    }

    public void Button_StartGameClicked()
    {
        ResetLevel();
        ManagerView.Instance.StartGame();
        StartCoroutine(GameTimer());
        StartCoroutine(ManagerView.Instance.GenerateBalls(7, 0.1f));
    }

    public void Button_NextLevelClicked()
    {

    }

    private void ResetLevel()
    {
        m_last_popped_bubble_type = ManagerView.BubbleColors.Black;
        m_bubbles_in_combo = 0;
        m_game_active = true;
        m_health = 1000;
        UpdateScore(0, true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) == false)
            m_mouse_went_up = true;

#elif UNITY_ANDROID
        if (Input.touchCount == 0)
            m_mouse_went_up = true;
#endif

    }

    public void Bomb(List<string> colliders_in_range)
    {
        for (int i = 0; i < colliders_in_range.Count; i++)
        {
            //might be that the same bubble is inside many colliders
            if (ManagerView.Instance.InList(colliders_in_range[i]))
            {
                ManagerView.Instance.KillObject(colliders_in_range[i]);
            }
        }
    }

    public void HandlePowerUps(Vector2 pos)
    {
        if (UnityEngine.Random.Range(0, 1f) > 0.8f)
        {
            float new_range = UnityEngine.Random.Range(0, 1f);
            //Debug.Log("new_range " + new_range);

            if (new_range < 0.3f)
            {
                ManagerView.Instance.GeneratePowerUp(1, pos);
            }
            else if (new_range >= 0.3f && new_range < 0.9f)
            {
                ManagerView.Instance.GeneratePowerUp(2, pos);
            }
            else if (new_range >= 0.9)
            {
                ManagerView.Instance.GeneratePowerUp(3, pos);
            }
        }
    }

    public bool BubblePopped(ManagerView.BubbleColors bubbleColors, Vector2 pos, BallView ballView)
    {
        if (Game_active == false)
            return false;

        if (m_special_power_up)
        {
            List<string> vs = new List<string>();

            ManagerView.Instance.AddMyTouchingRecursive(ballView, vs);

            for (int i = 0; i < vs.Count; i++)
                ManagerView.Instance.KillObject(vs[i]);

            m_special_power_up = false;
        }
        else
        {
            if (bubbleColors == m_last_popped_bubble_type)
                m_bubbles_in_combo++;
            else
                m_bubbles_in_combo = 0;

            if (m_bubbles_in_combo == 0 && m_mouse_went_up == false)
            {
                //need to put something totally new in the last poped bubble so it wont go in the condition next frame as a combo
                m_last_popped_bubble_type = ManagerView.BubbleColors.Black;
                //will not kill the bubble
                return false;
            }
            else
            {
                m_last_popped_bubble_type = bubbleColors;
                m_mouse_went_up = false;
                ManagerView.Instance.GenerateRandomBubble(true);

                HandlePowerUps(pos);

                int add_to_score = 10;

                if (m_bubbles_in_combo >= 3)
                    add_to_score += (m_bubbles_in_combo) * 2;

                UpdateScore(add_to_score);
                return true;
            }
        }
        return false;
    }

    public void HealthUp()
    {
        m_health += 20;
    }

    public void KillAll()
    {
        m_special_power_up = true;
    }

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

    

    private void UpdateScore(int added_to_score, bool reset = false)
    {
        if (reset)
            m_score = 0;
        else
            m_score += added_to_score;

        //Debug.Log("Score Added: " + added_to_score);

        ManagerView.Instance.Score_text.text = "SCORE: " + m_score;

    }
    private void TimerExpired()
    {
        ManagerView.Instance.TimerExpired();
    }

}
