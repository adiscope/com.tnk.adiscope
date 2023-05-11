#if UNITY_IOS

using UnityEditor.iOS.Xcode;

namespace Adiscope.Extension {
    public static class Extension {

        public static bool IsExistPlist(this PlistElementArray array, string value) {
            foreach(PlistElement elem in array.values) {
                if (elem.AsString() != null && elem.AsString().Equals(value)) {
                    return true;
                }
            }
            return false;
        }
    }
}

#endif