using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerView : MonoBehaviour
{

    [SerializeField] RectTransform m_rect;
    [SerializeField] Text m_text;
    int m_bubble_type;
    Action<List<string>> m_pop_action_with_param;
    Action m_pop_action_no_param;

    List<string> m_my_touching = new List<string>();

    public void SetData1(int bubble_type, Vector3 pos, Action<List<string>> bubble_pop_action_with_param, Action bubble_pop_action_no_param)
    {
        m_bubble_type = bubble_type;
        m_text.text = "B";

        if (bubble_type == 2)
            m_text.text = "H";

        if (bubble_type == 3)
            m_text.text = "S";


        m_rect.localPosition = pos;
        m_pop_action_with_param = bubble_pop_action_with_param;
        m_pop_action_no_param = bubble_pop_action_no_param;

    }

    public List<string> InMyRange()
    {
        List<string> m_in_range = new List<string>();

        foreach (Transform T in ManagerView.Instance.Balls_holder.transform)
        {
            RectTransform other_rect = T.GetComponent<RectTransform>();
            if (T.gameObject.GetComponent<BallView>() != null)
            {
                //compare distancces and see if its in range based on both objects radius
                double xpower = (m_rect.localPosition.x - other_rect.localPosition.x) * (m_rect.localPosition.x - other_rect.localPosition.x);
                double ypower = (m_rect.localPosition.y - other_rect.localPosition.y) * (m_rect.localPosition.y - other_rect.localPosition.y);

                double distance = Math.Sqrt(xpower + ypower);

                float other_raidus = other_rect.rect.width / 2;
                //double distance for the powerup
                float curr_radius = m_rect.rect.width;

                if (distance < other_raidus + curr_radius)
                    m_in_range.Add(T.gameObject.name);
            }
            
        }

        return m_in_range;
    }
    

    public void Button_Clicked()
    {
        GameController.Instance.Bubbles_can_click = false;

        if (m_bubble_type == 1)
            m_pop_action_with_param(InMyRange());
        else
            m_pop_action_no_param();

        DestroyImmediate(gameObject);
    }

    
}
