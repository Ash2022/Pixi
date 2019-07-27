using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    public static ModelManager Instance;
    List<Level> levels = new List<Level>();

    private void Awake()
    {
        Instance = this;
        levels.Add(new Level(1000, 50, 3, 90, 150, 250));
        levels.Add(new Level(1000, 50, 3, 75, 150, 250));
        levels.Add(new Level(1000, 50, 4, 50, 150, 300));
        levels.Add(new Level(1000, 50, 5, 50, 100, 350));
        levels.Add(new Level(1000, 50, 6, 50, 100, 400));

    }

    public class Level
    {
        int m_health;
        int m_decrease_rate;
        int m_num_colors;
        int m_min_size;
        int m_max_size;
        int m_pass_score;

        public Level(int m_health,int m_decrease_rate, int m_num_colors, int m_min_size, int m_max_size, int m_pass_score)
        {
            this.m_health = m_health;
            this.m_decrease_rate = m_decrease_rate;
            this.m_num_colors = m_num_colors;
            this.m_min_size = m_min_size;
            this.m_max_size = m_max_size;
            this.m_pass_score = m_pass_score;
        }
    }

}
