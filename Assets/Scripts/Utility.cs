using UnityEngine;

public static class Utility
{
    public static float GetRandomValue(Vector2 v)
    {
        return Fract(Mathf.Sin(Vector2.Dot(v, new Vector2(12.9898f, 78.233f)) * 43758.5453123f));
    }

    public static Vector3 GetRandomPoint(Vector3 origin, float radius, bool fromShell)
    {
        float radians = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;

        if (!fromShell)
        {
            radius = UnityEngine.Random.Range(0, radius);
        }

        float x = origin.x + Mathf.Cos(radians) * radius;
        float y = origin.y + Mathf.Sin(radians) * radius;

        return new Vector3(x, y, origin.z);
    }

    public static Vector3 GetRandomPoint(Vector3 min, Vector3 max)
    {
        float positionX = UnityEngine.Random.Range(min.x, max.x);
        float positionY = UnityEngine.Random.Range(min.y, max.y);
        float positionZ = UnityEngine.Random.Range(min.z, max.z);

        return new Vector3(positionX, positionY, positionZ);
    }

    public static Vector2 GetRandomPoint(Vector2 v)
    {
        float dot1 = Vector2.Dot(v, new Vector2(127.1f, 311.7f));
        float dot2 = Vector2.Dot(v, new Vector2(269.5f, 183.3f));

        return Fract(new Vector2(dot1, dot2) * 43.5453f);
    }

    public static float GetNoiseValue(Vector2 v)
    {
        Vector2 i = Floor(v);
        Vector2 f = Fract(v);

        float a = GetRandomValue(i);
        float b = GetRandomValue(i + new Vector2(1.0f, 0.0f));
        float c = GetRandomValue(i + new Vector2(0.0f, 1.0f));
        float d = GetRandomValue(i + new Vector2(1.0f, 1.0f));

        Vector2 u = f * f * (3 * f);

        return Mathf.Lerp(a, b, u.x) + (c - a) * u.y * (1.0f - u.x) + (d - b) * u.x * u.y;
    }

    public static float GetFBMValue(Vector2 v, float persistence, int octaves)
    {
        float total = 0.0f;

        for (int i = 0; i < octaves; i++)
        {
            float freq = Mathf.Pow(2.0f, i);
            float amp = Mathf.Pow(persistence, i);

            total += GetNoiseValue(v * freq) * amp;
        }

        return total;
    }

    private static Vector2 GetWorleyCell(Vector2 v, int gridSize)
    {
        int x = Mathf.FloorToInt(v.x / gridSize);
        int z = Mathf.FloorToInt(v.y / gridSize);

        return new Vector2(x, z);
    }

    private static Vector2 GetWorleyPoint(Vector2 cell)
    {
        Vector2 point = GetRandomPoint(cell);

        return cell + point;
    }

    private static Vector2[] GetWorleyPoints(Vector2 v, int gridSize)
    {
        Vector2[] worleyPoints = new Vector2[9];
        Vector2 cell = GetWorleyCell(v, gridSize);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector2 nearbyCell = new Vector2(cell.x + (x - 1), cell.y + (y - 1));
                Vector2 worleyPoint = GetWorleyPoint(nearbyCell);

                worleyPoints[x + 3 * y] = worleyPoint;
            }
        }

        return worleyPoints;
    }

    private static Vector2 GetWorleyVector(Vector2 v, Vector2[] worleyPoints, int gridSize)
    {
        Vector2 minVector = new Vector2(1, 1) * Mathf.Infinity;
        float minDistance = minVector.magnitude;

        for (int i = 0; i < 9; i++)
        {
            Vector2 vector = worleyPoints[i] - (v / gridSize);

            float distance = vector.magnitude;

            if (distance < minDistance)
            {
                minVector = vector;
                minDistance = distance;
            }
        }

        return minVector;
    }

    private static Vector2 GetWorleyVector(Vector2 v, int gridSize)
    {
        Vector2[] worleyPoints = GetWorleyPoints(v, gridSize);
        Vector2 worleyVector = GetWorleyVector(v, worleyPoints, gridSize);

        return worleyVector;
    }

    public static float GetWorleyValue(Vector2 v, int gridSize, float steepness)
    {
        Vector2 worleyVector = GetWorleyVector(v, gridSize);
        float worleyValue = worleyVector.magnitude;
        worleyValue = steepness * worleyValue * worleyValue * worleyValue;
        worleyValue = Mathf.Clamp01(worleyValue);

        return 1 - worleyValue;
    }

    public static float Fract(float x)
    {
        return x - Mathf.Floor(x);
    }

    public static Vector2 Fract(Vector2 v)
    {
        return new Vector2(Fract(v.x), Fract(v.y));
    }

    public static Vector2 Floor(Vector2 v)
    {
        return new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
    }

    public static int Wraparound(int i, int count)
    {
        if (i >= count)
        {
            return 0;
        }
        else if (i < 0)
        {
            return count - 1;
        }
        else
        {
            return i;
        }
    }
}