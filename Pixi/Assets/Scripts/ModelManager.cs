using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    public static ModelManager Instance;
    List<Level> levels = new List<Level>();

    public List<Level> Levels { get => levels; set => levels = value; }

    private void Awake()
    {
        Instance = this;
        levels.Add(new Level(10, 3, 60, 80, 250));
        levels.Add(new Level(10, 3, 75, 150, 250));
        levels.Add(new Level(10, 4, 50, 150, 300));
        levels.Add(new Level(10, 5, 50, 100, 350));
        levels.Add(new Level(10, 6, 50, 100, 400));

    }

    public class Level
    {
        int m_num_turns;
        int m_num_colors;
        int m_min_size;
        int m_max_size;
        int m_pass_score;

        public Level(int m_num_turns, int m_num_colors, int m_min_size, int m_max_size, int m_pass_score)
        {
            this.m_num_turns = m_num_turns;
            this.m_num_colors = m_num_colors;
            this.m_min_size = m_min_size;
            this.m_max_size = m_max_size;
            this.m_pass_score = m_pass_score;
        }

        public int Num_colors { get => m_num_colors; set => m_num_colors = value; }
        public int Num_turns { get => m_num_turns; set => m_num_turns = value; }

        public Vector2 GetSizeRange()
        {
            return new Vector2(m_min_size, m_max_size);
        }
    }

}
