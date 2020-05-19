using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    public class RoadController : MonoBehaviour
    {
        public Transform trans1;
        public Transform trans2;

        LineRenderer render;

        // Use this for initialization
        void Start()
        {
            render = GetComponent<LineRenderer>();
            render.positionCount = 2;
            render.SetPosition(0, trans1.localPosition);
            render.SetPosition(1, trans2.localPosition);
            render.sortingLayerName = SortingLayerName.EFFECT;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
