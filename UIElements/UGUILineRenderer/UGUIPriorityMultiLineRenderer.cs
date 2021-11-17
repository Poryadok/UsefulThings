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
            None,
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

        private float tailUvEnd;
        private float bodyUvStart;
        private Vector2 UV_TOP_LEFT;
        private Vector2 UV_BOTTOM_LEFT;
        private Vector2 UV_BOTTOM_RIGHT;
        private Vector2 UV_TOP_RIGHT;

        private Vector2[] fullUvs;
        private Vector2[] startUvs;

        public Sprite Tail;
        public Sprite Body;

        public bool UseColor = true;

        private Texture _texture;

        public override Texture mainTexture => _texture;

        [SerializeField]
        private Rect _UVRect = new Rect(0f, 0f, 1f, 1f);
        [SerializeField]
        private List<PriorityLine> _lines = new List<PriorityLine>();

        protected override void Start()
        {
            base.Start();
            UpdateTextures();
        }

        public void UpdateTextures()
        {
            var width = (Body != null ? Body.texture.width : 0) + (Tail != null ? Tail.texture.width : 0);
            var height = (Body != null ? Body.texture.height : 0);
            var texture = new Texture2D(width, height);
            var bodyTextureStart = Tail != null ? Tail.texture.width : 0;
            texture.wrapMode = TextureWrapMode.Clamp;

            if (Tail != null)
            {
                texture.SetPixels(0, 0, Tail.texture.width, Tail.texture.height, Tail.texture.GetPixels());
            }
            texture.SetPixels(bodyTextureStart, 0, Body.texture.width, Body.texture.height, Body.texture.GetPixels());
            texture.Apply();

            CalculateUvs();

            _texture = texture;
            SetAllDirty();
        }

        private void CalculateUvs()
        {
            var tailLength = Tail != null ? Tail.texture.width : 0;
            tailUvEnd = (float)tailLength / (tailLength + Body.texture.width);
            bodyUvStart = tailUvEnd;
            UV_TOP_LEFT = new Vector2(tailUvEnd, 0);
            UV_BOTTOM_LEFT = new Vector2(tailUvEnd, 1);
            UV_BOTTOM_RIGHT = new Vector2(1, 1);
            UV_TOP_RIGHT = new Vector2(1, 0);

            fullUvs = new[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
            startUvs = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(tailUvEnd, 1), new Vector2(tailUvEnd, 0) };
        }

        [System.Serializable]
        public class PriorityLine
        {
            public int Priority;
            public Color Color;
            public List<Vector2> Points = new List<Vector2>();
            public bool IsStarting = false;
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

                var color = UseColor ? line.Color : line.Color + Color.white / 2;
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

                float tileLength = Body == null ? 1 : Body.rect.width / Body.rect.height * Mathf.Max(StartThickness, EndThickness);

                var uvOffset = bodyUvStart;
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

                    var drawnLength = 0f;

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
                            uvOffset = bodyUvStart;
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

                        var startthickness = isCalcThickness ? Mathf.Lerp(StartThickness, EndThickness, ((pointsToDistance[i - 1] + drawnLength) / distance)) : StartThickness;
                        drawnLength += Vector2.Distance(start, end);
                        var endthickness = isCalcThickness ? Mathf.Lerp(StartThickness, EndThickness, ((pointsToDistance[i - 1] + drawnLength) / distance)) : StartThickness;

                        if (LineCaps && line.IsStarting && i == 1 && segments.Count == 0)
                        {
                            segments.Add(CreateLineCap(start, end, startthickness, endthickness, SegmentType.Start, color));
                        }

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
                    if (LineJoins != JoinType.None)
                    {
                        if (i < lineSegments.Count - 1)
                        {
                            var leftUiVert = lineSegments[i].Last();
                            var rightUiVert = lineSegments[i + 1].First();

                            var vec1 = leftUiVert[1].position - leftUiVert[2].position;
                            var vec2 = rightUiVert[2].position - rightUiVert[1].position;
                            var angle = Vector2.Angle(vec1, vec2) * Mathf.Deg2Rad;

                            // Positive sign means the line is turning in a 'clockwise' direction
                            var sign = Mathf.Sign(Vector3.Cross(vec1.normalized, vec2.normalized).z);

                            // Calculate the miter point
                            var miterDistance = isCalcThickness ? StartThickness : Mathf.Lerp(0, distance, pointsToDistance[i + 1]) / (2 * Mathf.Tan(angle / 2));
                            var miterPointA = leftUiVert[2].position - vec1.normalized * miterDistance * sign;
                            var miterPointB = leftUiVert[3].position + vec1.normalized * miterDistance * sign;

                            if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_BEVEL_NICE_JOIN)
                            {
                                if (sign < 0)
                                {
                                    leftUiVert[2].position = miterPointA;
                                    rightUiVert[1].position = miterPointA;
                                }
                                else
                                {
                                    leftUiVert[3].position = miterPointB;
                                    rightUiVert[0].position = miterPointB;
                                }
                            }

                            var join = new UIVertex[] { leftUiVert[3], leftUiVert[2], rightUiVert[1], rightUiVert[0] };
                            vh.AddUIVertexQuad(join);
                        }
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

        private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, float startTickness, float endThickness, SegmentType type, Color color)
        {
            if (type == SegmentType.Start)
            {
                var capStart = start - ((end - start).normalized * startTickness / 3);
                return CreateLineSegment(capStart, start, EndThickness, startTickness, startUvs, color);
            }
            //else if (type == SegmentType.End)
            //{
            //    var capEnd = end + ((end - start).normalized * endThickness / 2);
            //    return CreateLineSegment(end, capEnd, startTickness, endThickness, SegmentType.End, color);
            //}

            Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
            return null;
        }

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

        private Vector2[] GetUVs(float start, float end)
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