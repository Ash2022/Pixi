using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerView : MonoBehaviour
{

    [SerializeField] RectTransform m_rect;
    [SerializeField] Text m_text;
    int m_power_type;
    Action<List<string>> m_power_action;

    List<string> m_my_touching = new List<string>();

    public void SetData1(int bubble_type, Vector3 pos, Action<List<string>> bubble_pop_action)
    {
        m_power_type = bubble_type;
        m_text.text = "B";

        if (bubble_type == 2)
            m_text.text = "H";

        if (bubble_type == 3)
            m_text.text = "S";


        m_rect.localPosition = pos;
        m_power_action = bubble_pop_action;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        m_my_touching.Add(other.gameObject.name);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        m_my_touching.Remove(other.gameObject.name);
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (!m_my_touching.Contains(other.gameObject.name))
            m_my_touching.Add(other.gameObject.name);
    }

    public void Button_Clicked()
    {
        m_power_action(m_my_touching);
        DestroyImmediate(gameObject);
    }

    
}
