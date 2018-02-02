using UnityEngine;
using UnityEditor;

public class RetroSkyboxEditor : MaterialEditor {
    public override void OnInspectorGUI() {
        serializedObject.Update();

        var theShader = serializedObject.FindProperty ("m_Shader"); 

        if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null) {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck()) {
                var dirPitch = GetMaterialProperty(targets, "_DirectionPitch");
                var dirYaw = GetMaterialProperty(targets, "_DirectionYaw");
                var sunPitch = GetMaterialProperty(targets, "_SunPitch");
                var sunYaw = GetMaterialProperty(targets, "_SunYaw");

                var dirPitchRad = dirPitch.floatValue * Mathf.Deg2Rad;
                var dirYawRad = dirYaw.floatValue * Mathf.Deg2Rad;
                var sunPitchRad = sunPitch.floatValue * Mathf.Deg2Rad;
                var sunYawRad = sunYaw.floatValue * Mathf.Deg2Rad;
                
                var direction = new Vector4(Mathf.Sin(dirPitchRad) * Mathf.Sin(dirYawRad), Mathf.Cos(dirPitchRad), 
                                            Mathf.Sin(dirPitchRad) * Mathf.Cos(dirYawRad), 0.0f);
                GetMaterialProperty(targets, "_Direction").vectorValue = direction;

                var sunDirection = new Vector4(Mathf.Sin(sunPitchRad) * Mathf.Sin(sunYawRad), Mathf.Cos(sunPitchRad), 
                                            Mathf.Sin(sunPitchRad) * Mathf.Cos(sunYawRad), 0.0f);
                GetMaterialProperty(targets, "_SunDirection").vectorValue = sunDirection;

                var sunDirectionP = new Vector4(Mathf.Sin(sunPitchRad + Mathf.PI/2.0f) * Mathf.Sin(sunYawRad), 
                                                Mathf.Cos(sunPitchRad + Mathf.PI/2.0f), 
                                                Mathf.Sin(sunPitchRad + Mathf.PI/2.0f) * Mathf.Cos(sunYawRad), 0.0f);
                GetMaterialProperty(targets, "_SunDirectionP").vectorValue = sunDirectionP;

                PropertiesChanged();
            }
        }
    }
}
