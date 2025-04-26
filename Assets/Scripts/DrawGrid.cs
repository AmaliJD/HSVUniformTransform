using UnityEngine;
using System.Collections.Generic;
using Unity.Hierarchy;

public class DrawGrid : MonoBehaviour
{
    [Min(0)] public float width = 17.77778f;
    [Min(0)] public float height = 10f;
    [Min(0)] public float scrollMult = .02f;

    Color[,] colors = new Color[48, 25];
    float numColumns;
    float numRows;

    int showCursorComparisonIndex = 1;
    const int maxCursorTypes = 8;

    public Vector2Int GetDimensions() => new Vector2Int((int)numColumns, (int)numRows);
    public Color[,] GetColors() => colors;
    int frame;
    bool flickerOn;

    float flickerTimer;
    const float FLICKER_TIME = .25f;

    public void LoadColors(SaveData saveData)
    {
        colors = new Color[saveData.dimensions.x, saveData.dimensions.y];
        numColumns = saveData.dimensions.x;
        numRows = saveData.dimensions.y;

        int index = 0;
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                Vector3 colorVector = saveData.colorsList[index++];
                Color color = new Color(colorVector.x, colorVector.y, colorVector.z);
                colors[i, j] = color;
            }
        }
    }

    public void SetColors()
    {
        numColumns = colors.GetLength(0);
        numRows = colors.GetLength(1);
        float h = 0, s = 1, v = 0;
        for (int i = 0; i < numRows; i++)
        {
            h = 0;
            for (int j = 0; j < numColumns; j++)
            {
                colors[j, i] = Color.HSVToRGB(h, s, v);
                h += 1 / numColumns;
            }

            if (v < 1)
            {
                v += 1 / Mathf.Floor(numRows / 2);
            }
            else
            {
                s -= 1 / Mathf.Floor(numRows / 2);
            }
        }
    }

    void Update()
    {
        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)((cursor.x + width / 2) * (numColumns / width));
        int y = (int)((cursor.y + height / 2) * (numRows / height));
        float cellWidth = width / numColumns;
        float cellHeight = height / numRows;
        Vector2 snappedCursor = new Vector2((-width / 2) + (cellWidth / 2), (-height / 2) + (cellHeight / 2)) + new Vector2(x * cellWidth, y * cellHeight);

        //if (Input.GetKeyDown(KeyCode.Space))
        //    SetColors();

        ScrollColor(x, y);

        GLGizmos.DrawSolid2DBoxArray(Vector2.zero, width, height, new Vector2(numColumns, numRows), colors);

        if (Input.GetKeyDown(KeyCode.Z))
            showCursorComparisonIndex = ++showCursorComparisonIndex % maxCursorTypes;

        if (showCursorComparisonIndex != 0 && (y >= 0 && y < numRows) && (x < numColumns && x >= 0))
        {
            float percentage = ((float)y) / (numRows - 1);
            float L = percentage * 100;
            float grayLightness = Mathf.Pow((L + 16) / 116, 3);
            Color grayColor = new Color(grayLightness, grayLightness, grayLightness);
            float rev = percentage > .5f ? -1 : 1;

            switch (showCursorComparisonIndex)
            {
                case 1:
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(cellWidth, cellHeight), .2f, grayColor);
                    break;
                case 2:
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(cellWidth, cellHeight), .2f, grayColor);
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(width * 2.5f, cellHeight), .2f, grayColor);
                    break;
                case 3:
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(cellWidth, cellHeight), .2f, grayColor);
                    GLGizmos.DrawSolidBox(snappedCursor + (Vector2.up * 1.5f * rev) + (Vector2.left * 1), Vector2.one * 2, grayColor);
                    GLGizmos.DrawSolidBox(snappedCursor + (Vector2.up * 1.5f * rev) - (Vector2.left * 1), Vector2.one * 2, colors[x, y]);
                    break;
                case 4:
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(cellWidth, cellHeight), .2f, grayColor);
                    GLGizmos.DrawSolid2DBoxArray(snappedCursor + (Vector2.up * 2f * rev), 4f, 3f, new Vector2(12, 9), new Color[2, 2] { { colors[x, y], grayColor }, { grayColor, colors[x, y] } });
                    break;
                case 5:
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(cellWidth, cellHeight), .2f, grayColor);
                    GLGizmos.DrawSolid2DBoxArray(snappedCursor + (Vector2.up * 2f * rev), 6f, 3f, new Vector2(72, 36), new Color[2, 2] { { colors[x, y], grayColor }, { grayColor, colors[x, y] } });
                    break;
                case 6:
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(cellWidth, cellHeight), .2f, grayColor);
                    GLGizmos.DrawSolidBox(snappedCursor + (Vector2.up * 2.75f * rev), Vector2.one * 4.25f, flickerOn ? grayColor : colors[x, y]);
                    break;
                case 7:
                    GLGizmos.DrawBoxRing(snappedCursor, new Vector2(cellWidth, cellHeight), .2f, grayColor);

                    float steps = 9;
                    float boxSize = .5f;
                    float gradientWidth = (boxSize * (steps-1)) / 2;
                    float offset = InverseLerp((numColumns - 1) / 2, numColumns - 1, x);
                    for (float i = 0; i < steps; i++)
                    {
                        float t = i / (steps - 1);
                        float tx = InverseLerp((steps - 1) / 2, steps - 1, i);
                        GLGizmos.DrawSolidBox(snappedCursor + (Vector2.up * (boxSize * 2 + .5f) * rev) + (Vector2.right * tx * gradientWidth) - (Vector2.right * offset * gradientWidth), new Vector2(boxSize, boxSize * 4), Color.Lerp(grayColor, colors[x, y], t));
                    }
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
            GetComponent<SaveLoad>().Save();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        flickerTimer += Time.deltaTime;
        if (flickerTimer > FLICKER_TIME)
        {
            flickerTimer = 0;
            flickerOn = !flickerOn;
        }
        Debug.Log(flickerTimer);

        frame++;
    }

    float InverseLerp(float a, float b, float value) => (value - a) / (b - a);

    void ScrollColor(int x, int y)
    {
        if (x >= numColumns || x < 0 || y >= numRows - 1 || y < 1)
            return;

        if (showCursorComparisonIndex == 0)
            return;

        float mouseScrollDelta = Input.mouseScrollDelta.y;
        float shiftAmt = mouseScrollDelta * scrollMult * (Input.GetKey(KeyCode.LeftShift) ? .05f : 1f);


        Color.RGBToHSV(colors[x, y], out float h, out float s, out float v);
        Color upperBoundColor = y < numRows - 1 ? colors[x, y + 1] : Color.white;
        Color lowerBoundColor = y > 0 ? colors[x, y - 1] : Color.black;
        Color.RGBToHSV(upperBoundColor, out float upperH, out float upperS, out float upperV);
        Color.RGBToHSV(lowerBoundColor, out float lowerH, out float lowerS, out float lowerV);

        if (Input.GetMouseButtonDown(0))
        {
            if (lowerV < 1 && upperV >= 1 && lowerS >= 1 && upperS < 1)
            {
                colors[x, y] = Color.HSVToRGB(h, 1, 1);
                //Debug.Log($"set to full hue: {s}, {v}");
            }
        }

        if (mouseScrollDelta == 0)
            return;

        if (v < 1)
        {
            //Debug.Log($"low v: {v}");
            if (v + shiftAmt < (upperV != 1 ? upperV : 2) && v + shiftAmt > lowerV)
                colors[x, y] = Color.HSVToRGB(h, s, Mathf.Clamp01(v + shiftAmt));
        }
        else if (s < 1)
        {
            //Debug.Log($"low s: {s}");
            if (s - shiftAmt > upperS && s - shiftAmt < (lowerS != 1 ? lowerS : 2))
                colors[x, y] = Color.HSVToRGB(h, Mathf.Clamp01(s - shiftAmt), v);
        }
        else
        {
            //Debug.Log($"medium: {s}, {v}");
            if (mouseScrollDelta < 0)
            {
                if (v + shiftAmt < (upperV != 1 ? upperV : 2) && v + shiftAmt > lowerV)
                    colors[x, y] = Color.HSVToRGB(h, s, Mathf.Clamp01(v + shiftAmt));
            }
            else
            {
                if (s - shiftAmt > upperS && s - shiftAmt < (lowerS != 1 ? lowerS : 2))
                    colors[x, y] = Color.HSVToRGB(h, Mathf.Clamp01(s - shiftAmt), v);
            }
        }
    }
}
