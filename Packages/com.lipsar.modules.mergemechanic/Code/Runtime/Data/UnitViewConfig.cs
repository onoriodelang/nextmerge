using UnityEngine;

namespace Modules.MergeMechanic.Data
{
    [CreateAssetMenu(menuName = "Modules/MergeMechanic/UnitViewConfig", fileName = "UnitViewConfig")]
    public class UnitViewConfig : ScriptableObject
    {
        public UnitView[] UnitViews;

        public ViewHub.ViewElement GetView(int TypeID, int currentLevel)
        {
            for (int i = 0; i < UnitViews.Length; i++)
            {
                if (UnitViews[i].TypeID == TypeID)
                    return UnitViews[i].LevelViews[currentLevel % UnitViews[i].LevelViews.Length];
            }
            return null;
        }

        public int GetMaxLevel(int TypeID)
        {
            for (int i = 0; i < UnitViews.Length; i++)
            {
                if (UnitViews[i].TypeID == TypeID)
                    return UnitViews[i].LevelViews.Length - 1;
            }

            return 0;
        }

        public ViewHub.ViewElement GetRootObject(int TypeID)
        {
            for (int i = 0; i < UnitViews.Length; i++)
            {
                if (UnitViews[i].TypeID == TypeID)
                    return UnitViews[i].RootObject;
            }
            return null;
        }
    }

    [System.Serializable]
    public struct UnitView
    {
        public int TypeID;
        public ViewHub.ViewElement RootObject;
        public ViewHub.ViewElement[] LevelViews;
    }
}
