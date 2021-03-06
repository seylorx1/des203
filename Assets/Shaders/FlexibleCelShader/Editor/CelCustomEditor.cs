﻿using UnityEngine;
using UnityEditor;

public class CelCustomEditor : MaterialEditor {
    float spacing = 20;

    bool isTransparent() {
        return
            GetMaterialProperty(targets, "_SrcBlend").floatValue == (float)UnityEngine.Rendering.BlendMode.SrcAlpha &&
            GetMaterialProperty(targets, "_DstBlend").floatValue == (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
    }

    void showProperty(string propertyName) {
        MaterialProperty property = GetMaterialProperty(targets, propertyName);
        ShaderProperty(property, property.displayName);
    }

    void showPropertyAsInt(string propertyName) {
        MaterialProperty property = GetMaterialProperty(targets, propertyName);
        ShaderProperty(property, property.displayName);
        property.floatValue = Mathf.Round(property.floatValue);
    }

    void showPropertyGreaterZero(string propertyName) {
        MaterialProperty property = GetMaterialProperty(targets, propertyName);
        ShaderProperty(property, property.displayName);
        if (property.floatValue < 0) {
            property.floatValue = 0;
        }
    }

    public override void OnInspectorGUI() {
        if (isVisible) {

            EditorGUILayout.LabelField("Base Color and Textures", EditorStyles.boldLabel);
            showProperty("_MainTex");
            showProperty("_NormalTex");
            showProperty("_EmmisTex");
            showProperty("_Color");

            //MaterialProperty zWrite = GetMaterialProperty(targets, "_ZWrite");
            if(UpdateTransparencySettings(EditorGUILayout.Toggle("Transparent Blending", isTransparent()))) { 
                //Set Render Queue
                string newRenderQueue = EditorGUILayout.TextField(new GUIContent("Render Queue"), ((Material)target).renderQueue.ToString());
                int newRenderQueueValue = 0;
                ((Material)target).renderQueue = int.TryParse(newRenderQueue, out newRenderQueueValue) ? newRenderQueueValue : ((Material)target).renderQueue;
            }

            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("Realtime Lighting", EditorStyles.boldLabel);
            showProperty("_ApplyPointLighting");
            if (GetMaterialProperty(targets, "_ApplyPointLighting").floatValue == 1.0f) {
                showProperty("_PointLightBanding");
                showProperty("_PointLightBlendMode");
                showProperty("_PointLightSaturation");
                showProperty("_PointLightScalar");
            }
            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("Cracks", EditorStyles.boldLabel);
            showProperty("_ApplyCracks");
            if (GetMaterialProperty(targets, "_ApplyCracks").floatValue == 1.0f) {
                showProperty("_CrackStartScale");
                showProperty("_CrackEndScale");
                showProperty("_CrackStartEndThickness");
                showProperty("_CrackAmount");
            }
            else {
                EditorGUILayout.LabelField("Only enable on breakable (Entity) objects.", EditorStyles.miniLabel);
            }
            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("Lighting Ramp", EditorStyles.boldLabel);
            showPropertyAsInt("_RampLevels");
            showProperty("_LightScalar");
            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("High Light", EditorStyles.boldLabel);
            showProperty("_HighColor");
            showProperty("_HighIntensity");
            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("Low Light", EditorStyles.boldLabel);
            showProperty("_LowColor");
            showProperty("_LowIntensity");
            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
            showProperty("_OutlineColor");
            showPropertyGreaterZero("_OutlineSize");
            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("Hard Edge Light", EditorStyles.boldLabel);
            showProperty("_RimColor");
            showProperty("_RimAlpha");
            showProperty("_RimPower");
            showProperty("_RimDropOff");
            GUILayout.Space(spacing);

            EditorGUILayout.LabelField("Soft Edge Light", EditorStyles.boldLabel);
            showProperty("_FresnelColor");
            showProperty("_FresnelBrightness");
            showProperty("_FresnelPower");
            showProperty("_FresnelShadowDropoff");
        }
    }

    private void RefreshTransparencySettings() {
        UpdateTransparencySettings(isTransparent());
    }


    /// <param name="transparent"></param>
    /// <returns>transparent param</returns>
    private bool UpdateTransparencySettings(bool transparent) {
        MaterialProperty srcBlend = GetMaterialProperty(targets, "_SrcBlend");
        MaterialProperty dstBlend = GetMaterialProperty(targets, "_DstBlend");
        Material targetMat = (Material)target;

        if (transparent) {
            srcBlend.floatValue = (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
            dstBlend.floatValue = (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;

            //Update Material Tags
            targetMat.SetOverrideTag("RenderType", "Transparent");
            targetMat.SetInt("_ZWrite", 0);

            //Set Render Queue
            if (targetMat.renderQueue < 3000) {
                targetMat.renderQueue = 3001;
            }

        }
        else {
            srcBlend.floatValue = (float)UnityEngine.Rendering.BlendMode.One;
            dstBlend.floatValue = (float)UnityEngine.Rendering.BlendMode.Zero;

            //Update Material Tags
            targetMat.SetOverrideTag("RenderType", "Opaque");
            targetMat.SetInt("_ZWrite", 1);

            //Set Render Queue
            targetMat.renderQueue = 2000;
        }

        return transparent;
    }
}

