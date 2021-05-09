using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.UI
{
    [AddComponentMenu("UI/Extensions/Primitives/UGUIPriorityMultiLineRenderer")]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UGUIPriorityMultiLineRenderer : MaskableGraphic
    {
        private enum SegmentType
        {
            Start,
            Middle,
            End,
        }

        public enum JoinType
        {
            Bevel,
            Miter,
            Round
        }
        public enum BezierType
        {
            None,
            Quick,
            Basic,
            Improved,
        }

        private const float MIN_MITER_JOIN = 15 * Mathf.Deg2Rad;

        // A bevel 'nice' join displaces the vertices of the line segment instead of simply rendering a
        // quad to connect the endpoints. This improves the look of textured and transparent lines, since
        // there is no overlapping.
        private const float MIN_BEVEL_NICE_JOIN = 30 * Mathf.Deg2Rad;

        private static readonly Vector2 UV_TOP_LEFT = Vector2.zero;
        private static readonly Vector2 UV_BOTTOM_LEFT = new Vector2(0, 1);
        private static readonly Vector2 UV_BOTTOM_RIGHT = new Vector2(1, 1);
        private static readonly Vector2 UV_TOP_RIGHT = new Vector2(1, 0);

        private static readonly Vector2[] fullUvs = new[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };

        public Sprite Image;

        public override Texture mainTexture => Image.texture;

        [SerializeField]
        private Rect _UVRect = new Rect(0f, 0f, 1f, 1f);
        [SerializeField]
        private List<PriorityLine> _lines = new List<PriorityLine>();

        [System.Serializable]
        public class PriorityLine
        {
            public int Priority;
            public Color Color;
            public List<Vector2> Points = new List<Vector2>();
            public bool IsEnding = true;

            public PriorityLine(int priority, Color color)
            {
                this.Priority = priority;
                this.Color = color;
            }
        }

        [SerializeField]
        private float _startThickness = 20;
        public float StartThickness
        {
            get
            {
                return _startThickness;
            }
            set
            {
                if (_startThickness == value)
                {
                    return;
                }
                _startThickness = value;
                SetVerticesDirty();
            }
        }
        [SerializeField]
        private float _endThickness = 20;
        public float EndThickness
        {
            get
            {
                return _endThickness;
            }
            set
            {
                if (_endThickness == value)
                {
                    return;
                }
                _endThickness = value;
                SetVerticesDirty();
            }
        }

        public bool UseMargins;
        public Vector2 Margin;
        public bool relativeSize;

        public bool LineList = false;
        public bool LineCaps = false;
        public JoinType LineJoins = JoinType.Bevel;

        public BezierType BezierMode = BezierType.None;
        public int BezierSegmentsPerCurve = 10;

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return _UVRect;
            }
            set
            {
                if (_UVRect == value)
                {
                    return;
                }

                _UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Points to be drawn in the line.
        /// </summary>
        public List<PriorityLine> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                if (_lines == value)
                {
                    return;
                }

                _lines = value;
                SetAllDirty();
            }
        }

        public void Simplify(PriorityLine line, float minGap = 50, float minAngle = 5)
        {
            if (line.Points.Count < 2)
            {
                return;
            }
            var newPoints = new List<Vector2>(line.Points.Count);

            newPoints.Add(line.Points[0]);
            var last = line.Points[0];

            for (int i = 1; i < line.Points.Count - 1; i++)
            {
                if (Vector2.Angle(line.Points[i] - last, line.Points[i + 1] - last) < minAngle && Vector2.Distance(line.Points[i], last) < minGap)
                {
                    continue;
                }
                newPoints.Add(line.Points[i]);
                last = line.Points[i];
            }

            newPoints.Add(line.Points.Last());

            line.Points = newPoints;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (_lines == null || _lines.Count == 0)
            {
                return;
            }

            Lines.Sort((x, y) => x.Priority > y.Priority ? 1 : -1);

            foreach (var line in Lines)
            {
                if (line.Points.Count < 2)
                {
                    continue;
                }

                var color = line.Color;
                var pointsToDraw = line.Points;
                //If Bezier is desired, pick the implementation
                //if (BezierMode != BezierType.None && m_points.Length > 3)
                //{
                //    BezierPath bezierPath = new BezierPath();

                //    bezierPath.SetControlPoints(pointsToDraw);
                //    bezierPath.SegmentsPerCurve = BezierSegmentsPerCurve;
                //    List<Vector2> drawingPoints;
                //    switch (BezierMode)
                //    {
                //        case BezierType.Basic:
                //            drawingPoints = bezierPath.GetDrawingPoints0();
                //            break;
                //        case BezierType.Improved:
                //            drawingPoints = bezierPath.GetDrawingPoints1();
                //            break;
                //        default:
                //            drawingPoints = bezierPath.GetDrawingPoints2();
                //            break;
                //    }
                //    pointsToDraw = drawingPoints.ToArray();
                //}

                var sizeX = rectTransform.rect.width;
                var sizeY = rectTransform.rect.height;
                var offsetX = 0f;//-rectTransform.pivot.x * rectTransform.rect.width;
                var offsetY = 0f; //-rectTransform.pivot.y * rectTransform.rect.height;

                // don't want to scale based on the size of the rect, so this is switchable now
                if (!relativeSize)
                {
                    sizeX = 1;
                    sizeY = 1;
                }

                if (UseMargins)
                {
                    sizeX -= Margin.x;
                    sizeY -= Margin.y;
                    offsetX += Margin.x / 2f;
                    offsetY += Margin.y / 2f;
                }

                var isCalcThickness = StartThickness != EndThickness && line.IsEnding;

                var distance = 0f;

                var pointsToDistance = new float[pointsToDraw.Count];

                if (isCalcThickness)
                {
                    for (int i = 1; i < pointsToDraw.Count; i++)
                    {
                        var sum = distance + Vector2.Distance(pointsToDraw[i], pointsToDraw[i - 1]);
                        pointsToDistance[i] = sum;
                        distance = sum;
                    }
                }

                var tileLength = Image.rect.width / Image.rect.height * Mathf.Max(StartThickness, EndThickness);

                var uvOffset = 0f;
                // Generate the quads that make up the wide line
                var lineSegments = new List<List<UIVertex[]>>();
                for (var i = 1; i < pointsToDraw.Count; i++)
                {
                    var segmentStart = pointsToDraw[i - 1];
                    var segmentEnd = pointsToDraw[i];

                    if (segmentStart == segmentEnd)
                    {
                        continue;
                    }

                    var segments = new List<UIVertex[]>();
                    lineSegments.Add(segments);

                    while (segmentStart != segmentEnd)
                    {
                        var start = segmentStart;
                        var end = segmentEnd;
                        float startUV;
                        float endUV;

                        if (Vector2.Distance(start, end) > tileLength * (1 - uvOffset))
                        {
                            end = start + (end - start).normalized * tileLength * (1 - uvOffset);
                            startUV = uvOffset;
                            endUV = 1f;
                            uvOffset = 0f;
                            segmentStart = end;
                        }
                        else
                        {
                            startUV = uvOffset;
                            endUV = uvOffset + Vector2.Distance(start, end) / tileLength;
                            uvOffset = endUV;
                            segmentStart = segmentEnd;
                        }

                        start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
                        end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);

                        var startthickness = isCalcThickness ? Mathf.Lerp(StartThickness, EndThickness, (pointsToDistance[i - 1] / distance)) : StartThickness;
                        var endthickness = isCalcThickness ? Mathf.Lerp(StartThickness, EndThickness, (pointsToDistance[i] / distance)) : StartThickness;

                        //if (LineCaps && i == 1)
                        //{
                        //    segments.Add(CreateLineCap(start, end, startthickness, endthickness, SegmentType.Start, color));
                        //}

                        segments.Add(CreateLineSegment(start, end, startthickness, endthickness, GetUVs(startUV, endUV), color));


                        //if (LineCaps && i == pointsToDraw.Count - 1)
                        //{
                        //    segments.Add(CreateLineCap(start, end, startthickness, endthickness, SegmentType.End, color));
                        //}
                    }
                }

                // Add the line segments to the vertex helper, creating any joins as needed
                for (var i = 0; i < lineSegments.Count; i++)
                {
                    if (i < lineSegments.Count - 1)
                    {
                        var vec1 = lineSegments[i].Last()[1].position - lineSegments[i].Last()[2].position;
                        var vec2 = lineSegments[i + 1].First()[2].position - lineSegments[i + 1].First()[1].position;
                        var angle = Vector2.Angle(vec1, vec2) * Mathf.Deg2Rad;

                        // Positive sign means the line is turning in a 'clockwise' direction
                        var sign = Mathf.Sign(Vector3.Cross(vec1.normalized, vec2.normalized).z);

                        // Calculate the miter point
                        var miterDistance = isCalcThickness ? StartThickness : Mathf.Lerp(0, distance, pointsToDistance[i + 1]) / (2 * Mathf.Tan(angle / 2));
                        var miterPointA = lineSegments[i].Last()[2].position - vec1.normalized * miterDistance * sign;
                        var miterPointB = lineSegments[i].Last()[3].position + vec1.normalized * miterDistance * sign;

                        var joinType = LineJoins;
                        if (joinType == JoinType.Miter)
                        {
                            // Make sure we can make a miter join without too many artifacts.
                            if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_MITER_JOIN)
                            {
                                lineSegments[i].Last()[2].position = miterPointA;
                                lineSegments[i].Last()[3].position = miterPointB;
                                lineSegments[i + 1].First()[0].position = miterPointB;
                                lineSegments[i + 1].First()[1].position = miterPointA;
                            }
                            else
                            {
                                joinType = JoinType.Bevel;
                            }
                        }

                        if (joinType == JoinType.Bevel)
                        {
                            if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_BEVEL_NICE_JOIN)
                            {
                                if (sign < 0)
                                {
                                    lineSegments[i].Last()[2].position = miterPointA;
                                    lineSegments[i + 1].First()[1].position = miterPointA;
                                }
                                else
                                {
                                    lineSegments[i].Last()[3].position = miterPointB;
                                    lineSegments[i + 1].First()[0].position = miterPointB;
                                }
                            }

                            var join = new UIVertex[] { lineSegments[i].Last()[2], lineSegments[i].Last()[3], lineSegments[i + 1].First()[0], lineSegments[i + 1].First()[1] };
                            vh.AddUIVertexQuad(join);
                        }

                        //if (joinType == JoinType.Round)
                        //{
                        //    // Tessellate an approximation of a circle
                        //    var center = new Vector2(pointsToDraw[i + 1].x * sizeX + offsetX, pointsToDraw[i + 1].y * sizeY + offsetY);
                        //    Vector2 v0 = center;
                        //    Vector2 uv0 = new Vector2(0, 0.5f);

                        //    if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_BEVEL_NICE_JOIN)
                        //    {
                        //        if (sign < 0)
                        //        {
                        //            segments[i][2].position = miterPointA;
                        //            segments[i + 1][1].position = miterPointA;
                        //            v0 = miterPointA;
                        //            uv0 = segments[i][2].uv0;
                        //        }
                        //        else
                        //        {
                        //            segments[i][3].position = miterPointB;
                        //            segments[i + 1][0].position = miterPointB;
                        //            v0 = miterPointB;
                        //            uv0 = segments[i][3].uv0;
                        //        }
                        //    }

                        //    var two = segments[i][3].position - (Vector3)center;
                        //    var one = segments[i + 1][0].position - (Vector3)center;
                        //    var uv1 = segments[i][3].uv0;
                        //    if (sign > 0)
                        //    {
                        //        one = segments[i][2].position - (Vector3)center;
                        //        two = segments[i + 1][1].position - (Vector3)center;
                        //        uv1 = segments[i][2].uv0;
                        //    }

                        //    var tesselation = 12;
                        //    List<UIVertex> verts = new List<UIVertex>();

                        //    var v1 = one;
                        //    for (var iteration = 0; iteration < tesselation; iteration++)
                        //    {
                        //        var v2 = Vector3.RotateTowards(v1, two, Mathf.PI / tesselation, 0.1f);
                        //        verts.AddRange(CreateTriangle(new[] { v0, center + (Vector2)v1, center + (Vector2)v2 }, new[] { uv0, uv1, uv1 }));
                        //        v1 = v2;
                        //    }
                        //    vh.AddUIVertexTriangleStream(verts);
                        //}
                    }
                    foreach (var uiVert in lineSegments[i])
                    {
                        vh.AddUIVertexQuad(uiVert);
                    }
                }
            }
        }

        private UIVertex[] CreateTriangle(Vector2[] vertices, Color color, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[3];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }

        //private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, float startTickness, float endThickness, SegmentType type, Color color)
        //{
        //    if (type == SegmentType.Start)
        //    {
        //        var capStart = start - ((end - start).normalized * startTickness / 2);
        //        return CreateLineSegment(capStart, start, startTickness, endThickness, SegmentType.Start, color);
        //    }
        //    else if (type == SegmentType.End)
        //    {
        //        var capEnd = end + ((end - start).normalized * endThickness / 2);
        //        return CreateLineSegment(end, capEnd, startTickness, endThickness, SegmentType.End, color);
        //    }

        //    Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
        //    return null;
        //}

        private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, float startTickness, float endThickness, Vector2[] uvs, Color color)
        {
            Vector2 startoffset = new Vector2(start.y - end.y, end.x - start.x).normalized * startTickness / 2;
            Vector2 endoffset = new Vector2(start.y - end.y, end.x - start.x).normalized * endThickness / 2;
            var v1 = start - startoffset;
            var v2 = start + startoffset;
            var v3 = end + endoffset;
            var v4 = end - endoffset;
            return SetVbo(new[] { v1, v2, v3, v4 }, color, uvs);
        }

        protected UIVertex[] SetVbo(Vector2[] vertices, Color color, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }

        private static Vector2[] GetUVs(float start, float end)
        {
            if (start == 0 && end == 1)
            {
                return fullUvs;
            }

            var result = new Vector2[4];

            if (start == 0)
            {
                result[0] = fullUvs[0];
                result[1] = fullUvs[1];
            }
            else
            {
                result[0] = new Vector2(start, 0);
                result[1] = new Vector2(start, 1);
            }

            if (end == 1)
            {
                result[2] = fullUvs[2];
                result[3] = fullUvs[3];
            }
            else
            {
                result[2] = new Vector2(end, 1);
                result[3] = new Vector2(end, 0);
            }

            return result;
        }

    }
}