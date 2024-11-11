using System;
using UnityEngine;

namespace Smart.Essence
{
    public class MediatorBindInfo
    {
        public readonly Type ControllerType;
        public readonly Type ViewType;
        public readonly MonoBehaviour View;

        public MediatorBindInfo(Type controllerType, Type viewType, MonoBehaviour view)
        {
            ControllerType = controllerType;
            ViewType = viewType;
            View = view;
        }
    }
}