using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using KS.SceneFusion.API;
using KS.Reactor;

#if SF_PROBUILDER
using ProBuilder2.EditorCommon;
#endif

namespace KS.SceneFusion.Extensions
{
    /**
     * Makes Scene Fusion and ProBuilder play nice together.
     * 
     * NOTE: If you get compilation errors because pb_Object is not defined, go to
     * Project Settings > Player > Scripting Define Symbols, remove SF_PROBUILDER and press enter.
     */
    [InitializeOnLoad]
    public class ProBuilderExtension
    {
#if SF_PROBUILDER
        private static HashSet<pb_Object> m_rebuiltObjects = new HashSet<pb_Object>();

        /**
         * Initialization
         */
        static ProBuilderExtension()
        {
            // Register property change handler
            sfObjectUtility.RegisterPropertyChangeHandler<pb_Object>(OnPropertyChange);

            // Register before spawn request handler
            sfObjectUtility.BeforeFirstSync += BeforeSpawnRequest;

            // Register on selection update handler. This is also invoked when geometry changes which is what we want
            // it for.
            pb_Editor.OnSelectionUpdate += OnGeometryChange;

            // Register on mesh compile handler.
            pb_EditorUtility.AddOnMeshCompiledListener(OnMeshCompiled);

            EditorApplication.update += Update;
        }

        /**
         * Called every frame.
         */
        private static void Update()
        {
            m_rebuiltObjects.Clear();
        }

        /**
         * Called before sending a spawn request for a game object to the server.
         * 
         * @param   GameObject gameObject a spawn request will be sent for.
         * @return  bool true - we always want to sync the object.
         */
        private static bool BeforeSpawnRequest(GameObject gameObject)
        {
            pb_Object obj = gameObject.GetComponent<pb_Object>();
            if (obj != null)
            {
                MeshFilter filter = gameObject.GetComponent<MeshFilter>();
                if (filter != null && filter.sharedMesh != null)
                {
                    // The game object has a mesh whose data is controlled by ProBuilder. Tell Scene Fusion not to
                    // upload the mesh data since Probuilder will be generating it.
                    sfObjectUtility.DontUploadSceneAsset(filter.sharedMesh);
                }
            }
            return true;
        }

        /**
         * Called when a property on a pb_Object is changed by another user. Rebuilds the ProBuilder mesh.
         * 
         * @param   SerializedProperty property that changed.
         */
        private static void OnPropertyChange(SerializedProperty property)
        {
            // Rebuild the mesh if we haven't already this frame.
            pb_Object obj = property.serializedObject.targetObject as pb_Object;
            if (obj != null && m_rebuiltObjects.Add(obj))
            {
                obj.ToMesh();
                obj.Refresh();
            }
        }

        /**
         * Called when probuilder changes geometry or the probuilder selection changes. Invalidates in-progress change
         * checks on the modified objects to ensure updates are atomic.
         * 
         * @param   pb_Object[] objects that changed.
         */
        private static void OnGeometryChange(pb_Object[] objects)
        {
            if (objects != null) // sometimes this is null, not sure why...
            {
                foreach (pb_Object obj in objects)
                {
                    sfObjectUtility.InvalidateChangeCheck(obj);
                }
            }
        }

        /**
         * Called when probuilder builds a mesh. Invalidates in-progress change checks on the modified object to ensure
         * updates are atomic.
         * 
         * @param   pb_Object obj whose mesh was built.
         * @param   Mesh mesh that was built.
         */
        private static void OnMeshCompiled(pb_Object obj, Mesh mesh)
        {
            sfObjectUtility.InvalidateChangeCheck(obj);
        }

#else
        /**
         * Initialization
         */
        static ProBuilderExtension()
        {
            // Set SF_ProBuilder define to compile ProBuilder-dependant code if ProBuilder is detected.
            if (DetectProBuilder())
            {
                sfUtility.SetDefineSymbol("SF_PROBUILDER");
            }
        }


        /**
         * Detects if ProBuilder 2.8+ is in the project.
         * 
         * @return  bool true if ProBuilder 2.8+ was detected.
         */
        private static bool DetectProBuilder()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType("ProBuilder2.EditorCommon.pb_EditorUtility");
                if (type != null)
                {
                    if (type.GetMethod("AddOnMeshCompiledListener") == null)
                    {
                        ksLog.Warning("Your version of ProBuilder is not compatible with Scene Fusion. Scene Fusion " +
                            "is compatible with Probuilder 2.8+.");
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
#endif
    }
}
