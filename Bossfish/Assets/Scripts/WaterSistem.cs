using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public static class WaterSistem 
{
    static WaterSurface water;
    static WaterSearchParameters searchParams = new WaterSearchParameters();

    // Отримує висоту хвилі в точці
    public static float SampleWaterHeight(Vector3 checkPos)
    {
        if (water == null)
        {
            water = Object.FindObjectOfType<WaterSurface>();
            if (water == null)
            {
                Debug.LogWarning("WaterSurface not found in the scene.");
                return checkPos.y;
            }
        }

        searchParams.targetPositionWS = checkPos;
        searchParams.startPositionWS = checkPos + Vector3.up * 5;

        if (water.ProjectPointOnWaterSurface(searchParams, out WaterSearchResult result))
        {
            return result.projectedPositionWS.y;
        }

        return checkPos.y;
    }
}
