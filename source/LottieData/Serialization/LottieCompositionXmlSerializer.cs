// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Toolkit.Uwp.UI.Lottie.LottieData.Serialization
{
#if PUBLIC_LottieData
    public
#endif
    sealed class LottieCompositionXmlSerializer
    {
        LottieCompositionXmlSerializer()
        {
        }

        public static XDocument ToXml(LottieObject lottieObject)
        {
            return new LottieCompositionXmlSerializer().ToXDocument(lottieObject);
        }

        XDocument ToXDocument(LottieObject lottieObject)
        {
            return new XDocument(FromLottieObject(lottieObject));
        }

        XElement FromLottieObject(LottieObject obj)
        {
            switch (obj.ObjectType)
            {
                case LottieObjectType.Ellipse:
                    return FromEllipse((Ellipse)obj);
                case LottieObjectType.ImageLayer:
                    return FromImageLayer((ImageLayer)obj);
                case LottieObjectType.LinearGradientFill:
                    return FromLinearGradientFill((LinearGradientFill)obj);
                case LottieObjectType.LinearGradientStroke:
                    return FromLinearGradientStroke((LinearGradientStroke)obj);
                case LottieObjectType.LottieComposition:
                    return FromLottieComposition((LottieComposition)obj);
                case LottieObjectType.Marker:
                    return FromMarker((Marker)obj);
                case LottieObjectType.MergePaths:
                    return FromMergePaths((MergePaths)obj);
                case LottieObjectType.NullLayer:
                    return FromNullLayer((NullLayer)obj);
                case LottieObjectType.Polystar:
                    return FromPolystar((Polystar)obj);
                case LottieObjectType.PreCompLayer:
                    return FromPreCompLayer((PreCompLayer)obj);
                case LottieObjectType.RadialGradientFill:
                    return FromRadialGradientFill((RadialGradientFill)obj);
                case LottieObjectType.RadialGradientStroke:
                    return FromRadialGradientStroke((RadialGradientStroke)obj);
                case LottieObjectType.Rectangle:
                    return FromRectangle((Rectangle)obj);
                case LottieObjectType.Repeater:
                    return FromRepeater((Repeater)obj);
                case LottieObjectType.RoundedCorner:
                    return FromRoundedCorner((RoundedCorner)obj);
                case LottieObjectType.Shape:
                    return FromShape((Path)obj);
                case LottieObjectType.ShapeGroup:
                    return FromShapeGroup((ShapeGroup)obj);
                case LottieObjectType.ShapeLayer:
                    return FromShapeLayer((ShapeLayer)obj);
                case LottieObjectType.SolidColorFill:
                    return FromSolidColorFill((SolidColorFill)obj);
                case LottieObjectType.SolidColorStroke:
                    return FromSolidColorStroke((SolidColorStroke)obj);
                case LottieObjectType.SolidLayer:
                    return FromSolidLayer((SolidLayer)obj);
                case LottieObjectType.TextLayer:
                    return FromTextLayer((TextLayer)obj);
                case LottieObjectType.Transform:
                    return FromTransform((Transform)obj);
                case LottieObjectType.TrimPath:
                    return FromTrimPath((TrimPath)obj);
            }

            throw Unreachable;
        }

        XElement FromLottieComposition(LottieComposition lottieComposition)
        {
            return new XElement("LottieComposition", GetContents());
            IEnumerable<XObject> GetContents()
            {
                yield return new XAttribute(nameof(lottieComposition.Version), lottieComposition.Version.ToString());
                if (!string.IsNullOrWhiteSpace(lottieComposition.Name))
                {
                    yield return new XAttribute(nameof(lottieComposition.Name), lottieComposition.Name);
                }

                yield return new XAttribute(nameof(lottieComposition.Width), lottieComposition.Width);
                yield return new XAttribute(nameof(lottieComposition.Height), lottieComposition.Height);
                yield return new XAttribute(nameof(lottieComposition.InPoint), lottieComposition.InPoint);
                yield return new XAttribute(nameof(lottieComposition.OutPoint), lottieComposition.OutPoint);
                yield return FromAssetCollection(lottieComposition.Assets);
                yield return FromLayerCollection(lottieComposition.Layers);
                if (lottieComposition.Markers.Any())
                {
                    yield return new XElement("Markers", lottieComposition.Markers.Select(FromMarker));
                }
            }
        }

        XElement FromAssetCollection(AssetCollection assets)
        {
            return new XElement("Assets", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var asset in assets)
                {
                    yield return FromAsset(asset);
                }
            }
        }

        XElement FromAsset(Asset asset)
        {
            switch (asset.Type)
            {
                case Asset.AssetType.LayerCollection:
                    return FromLayersAsset((LayerCollectionAsset)asset);
                case Asset.AssetType.Image:
                    return FromImageAsset((ImageAsset)asset);
                default:
                    throw Unreachable;
            }
        }

        XElement FromLayersAsset(LayerCollectionAsset asset)
        {
            return new XElement(
                nameof(LayerCollectionAsset),
                new XAttribute(nameof(asset.Id), asset.Id),
                FromLayerCollection(asset.Layers));
        }

        XElement FromImageAsset(ImageAsset asset)
        {
            switch (asset.ImageType)
            {
                case ImageAsset.ImageAssetType.Embedded:
                    return FromEmbeddedImageAsset((EmbeddedImageAsset)asset);
                case ImageAsset.ImageAssetType.External:
                    return FromExternalImageAsset((ExternalImageAsset)asset);
                default:
                    throw Unreachable;
            }
        }

        XElement FromEmbeddedImageAsset(EmbeddedImageAsset imageAsset)
        {
            return new XElement(
                nameof(ImageAsset),
                new XAttribute(nameof(imageAsset.Id), imageAsset.Id),
                new XAttribute(nameof(imageAsset.Width), imageAsset.Width),
                new XAttribute(nameof(imageAsset.Height), imageAsset.Height),
                new XAttribute(nameof(imageAsset.Format), imageAsset.Format),
                new XAttribute("SizeInBytes", imageAsset.Bytes.Length));
        }

        XElement FromExternalImageAsset(ExternalImageAsset imageAsset)
        {
            return new XElement(
                nameof(ImageAsset),
                new XAttribute(nameof(imageAsset.Id), imageAsset.Id),
                new XAttribute(nameof(imageAsset.Width), imageAsset.Width),
                new XAttribute(nameof(imageAsset.Height), imageAsset.Height),
                new XAttribute(nameof(imageAsset.Path), imageAsset.Path),
                new XAttribute(nameof(imageAsset.FileName), imageAsset.FileName));
        }

        XElement FromLayerCollection(LayerCollection layers)
        {
            return new XElement("Layers", GetContents());
            IEnumerable<XElement> GetContents()
            {
                foreach (var layer in layers.GetLayersBottomToTop().Reverse())
                {
                    yield return FromLayer(layer);
                }
            }
        }

        XElement FromLayer(Layer layer)
        {
            switch (layer.Type)
            {
                case Layer.LayerType.PreComp:
                    return FromPreCompLayer((PreCompLayer)layer);
                case Layer.LayerType.Solid:
                    return FromSolidLayer((SolidLayer)layer);
                case Layer.LayerType.Image:
                    return FromImageLayer((ImageLayer)layer);
                case Layer.LayerType.Null:
                    return FromNullLayer((NullLayer)layer);
                case Layer.LayerType.Shape:
                    return FromShapeLayer((ShapeLayer)layer);
                case Layer.LayerType.Text:
                    return FromTextLayer((TextLayer)layer);
                default:
                    throw Unreachable;
            }
        }

        IEnumerable<XObject> GetLayerContents(Layer layer)
        {
            yield return new XAttribute(nameof(layer.Index), layer.Index);
            foreach (var item in GetLottieObjectContents(layer))
            {
                yield return item;
            }

            if (layer.IsHidden)
            {
                yield return new XAttribute(nameof(layer.IsHidden), layer.IsHidden);
            }

            yield return new XAttribute(nameof(layer.StartTime), layer.StartTime);
            yield return new XAttribute(nameof(layer.InPoint), layer.InPoint);
            yield return new XAttribute(nameof(layer.OutPoint), layer.OutPoint);
            if (layer.TimeStretch != 1)
            {
                yield return new XAttribute(nameof(layer.TimeStretch), layer.TimeStretch);
            }

            if (layer.Parent.HasValue)
            {
                yield return new XAttribute(nameof(layer.Parent), layer.Parent.Value);
            }

            yield return FromTransform(layer.Transform);
            if (layer.Masks != null)
            {
                for (var i = 0; i < layer.Masks.Length; i++)
                {
                    yield return FromMask(layer.Masks[i]);
                }
            }

            yield return new XAttribute(nameof(layer.LayerMatteType), layer.LayerMatteType);
        }

        XElement FromPreCompLayer(PreCompLayer layer)
        {
            return new XElement("PreComp", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetLayerContents(layer))
                {
                    yield return item;
                }

                yield return new XAttribute(nameof(layer.Width), layer.Width);
                yield return new XAttribute(nameof(layer.Height), layer.Height);
                if (!string.IsNullOrWhiteSpace(layer.RefId))
                {
                    yield return new XAttribute(nameof(layer.RefId), layer.RefId);
                }
            }
        }

        XElement FromSolidLayer(SolidLayer layer)
        {
            return new XElement("Solid", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetLayerContents(layer))
                {
                    yield return item;
                }

                yield return new XAttribute(nameof(layer.Width), layer.Width);
                yield return new XAttribute(nameof(layer.Height), layer.Height);
                yield return new XAttribute(nameof(layer.Color), layer.Color);
            }
        }

        XElement FromImageLayer(ImageLayer layer)
        {
            return new XElement("Image", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetLayerContents(layer))
                {
                    yield return item;
                }

                if (!string.IsNullOrWhiteSpace(layer.RefId))
                {
                    yield return new XAttribute(nameof(layer.RefId), layer.RefId);
                }
            }
        }

        XElement FromNullLayer(NullLayer layer)
        {
            return new XElement("Null", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetLayerContents(layer))
                {
                    yield return item;
                }
            }
        }

        XElement FromShapeLayer(ShapeLayer layer)
        {
            return new XElement("Shape", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetLayerContents(layer))
                {
                    yield return item;
                }

                for (var i = 0; i < layer.Contents.Length; i++)
                {
                    yield return FromShapeLayerContent(layer.Contents[i]);
                }
            }
        }

        XElement FromTextLayer(TextLayer layer)
        {
            return new XElement("Text", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetLayerContents(layer))
                {
                    yield return item;
                }
            }
        }

        XElement FromShapeLayerContent(ShapeLayerContent content)
        {
            switch (content.ContentType)
            {
                case ShapeContentType.Group:
                    return FromShapeGroup((ShapeGroup)content);
                case ShapeContentType.SolidColorStroke:
                    return FromSolidColorStroke((SolidColorStroke)content);
                case ShapeContentType.LinearGradientStroke:
                    return FromLinearGradientStroke((LinearGradientStroke)content);
                case ShapeContentType.RadialGradientStroke:
                    return FromRadialGradientStroke((RadialGradientStroke)content);
                case ShapeContentType.SolidColorFill:
                    return FromSolidColorFill((SolidColorFill)content);
                case ShapeContentType.LinearGradientFill:
                    return FromLinearGradientFill((LinearGradientFill)content);
                case ShapeContentType.RadialGradientFill:
                    return FromRadialGradientFill((RadialGradientFill)content);
                case ShapeContentType.Transform:
                    return FromTransform((Transform)content);
                case ShapeContentType.Path:
                    return FromPath((Path)content);
                case ShapeContentType.Ellipse:
                    return FromEllipse((Ellipse)content);
                case ShapeContentType.Rectangle:
                    return FromRectangle((Rectangle)content);
                case ShapeContentType.Polystar:
                    return FromPolystar((Polystar)content);
                case ShapeContentType.TrimPath:
                    return FromTrimPath((TrimPath)content);
                case ShapeContentType.MergePaths:
                    return FromMergePaths((MergePaths)content);
                case ShapeContentType.Repeater:
                    return FromRepeater((Repeater)content);
                case ShapeContentType.RoundedCorner:
                    return FromRoundedCorner((RoundedCorner)content);
                default:
                    throw Unreachable;
            }
        }

        XElement FromMask(Mask mask)
        {
            return new XElement("Mask", GetContents());
            IEnumerable<XObject> GetContents()
            {
                yield return new XAttribute(nameof(mask.Inverted), mask.Inverted);
                yield return new XAttribute(nameof(mask.Name), mask.Name);
                yield return FromAnimatable(nameof(mask.Points), mask.Points);
                yield return FromAnimatable(nameof(mask.Opacity), mask.Opacity);
                yield return new XAttribute(nameof(mask.Mode), mask.Mode);
            }
        }

        XElement FromShapeGroup(ShapeGroup content)
        {
            return new XElement("Group", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                for (var i = 0; i < content.Contents.Length; i++)
                {
                    yield return FromShapeLayerContent(content.Contents[i]);
                }
            }
        }

        XElement FromSolidColorStroke(SolidColorStroke content)
        {
            return new XElement("SolidColorStroke", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable(nameof(content.Color), content.Color);
                yield return FromAnimatable(nameof(content.Opacity), content.Opacity);
                yield return FromAnimatable(nameof(content.StrokeWidth), content.StrokeWidth);
            }
        }

        XElement FromLinearGradientStroke(LinearGradientStroke content)
        {
            return new XElement("LinearGradientStroke", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        XElement FromRadialGradientStroke(RadialGradientStroke content)
        {
            return new XElement("RadialGradientStroke", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        XElement FromSolidColorFill(SolidColorFill content)
        {
            return new XElement("SolidColorFill", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable("Color", content.Color);
                yield return FromAnimatable("Opacity", content.Opacity);
            }
        }

        XElement FromLinearGradientFill(LinearGradientFill content)
        {
            return new XElement("LinearGradientFill", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable("GradientStops", content.GradientStops);
            }
        }

        XElement FromRadialGradientFill(RadialGradientFill content)
        {
            return new XElement("RadialGradientFill", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        XElement FromTransform(Transform content)
        {
            return new XElement("Transform", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable(nameof(content.ScalePercent), content.ScalePercent);
                yield return FromAnimatable(nameof(content.Position), content.Position);
                yield return FromAnimatable(nameof(content.Anchor), content.Anchor);
                yield return FromAnimatable(nameof(content.Opacity), content.Opacity);
                yield return FromAnimatable(nameof(content.Rotation), content.Rotation);
            }
        }

        XObject FromAnimatable(string name, IAnimatableVector3 animatable)
        {
            switch (animatable.Type)
            {
                case AnimatableVector3Type.Vector3:
                    return FromAnimatable<Vector3>(name, (AnimatableVector3)animatable);
                case AnimatableVector3Type.XYZ:
                    {
                        var xyz = (AnimatableXYZ)animatable;
                        return new XElement(
                            name,
                            FromAnimatable(nameof(xyz.X), xyz.X),
                            FromAnimatable(nameof(xyz.Y), xyz.Y),
                            FromAnimatable(nameof(xyz.Z), xyz.Z));
                    }

                default:
                    throw Unreachable;
            }
        }

        XObject FromAnimatable<T>(string name, Animatable<T> animatable)
            where T : IEquatable<T>
        {
            if (!animatable.IsAnimated)
            {
                return new XAttribute(name, $"{animatable.InitialValue}");
            }
            else
            {
                var keyframesString = string.Join(", ", animatable.KeyFrames.SelectToSpan(kf => $"{FromKeyFrame(kf)}").ToArray());

                return new XElement(name, keyframesString);
            }
        }

        static string FromKeyFrame<T>(KeyFrame<T> keyFrame)
            where T : IEquatable<T>
        {
            if (keyFrame is KeyFrame<Vector3>)
            {
                var v3kf = (KeyFrame<Vector3>)(object)keyFrame;
                var cp1 = v3kf.SpatialControlPoint1;
                var cp2 = v3kf.SpatialControlPoint2;
                if (cp1 != Vector3.Zero || cp2 != Vector3.Zero)
                {
                    // Spatial bezier
                    return $"SpatialBezier:{keyFrame.Value},{cp1},{cp2}@{keyFrame.Frame}({keyFrame.Easing.Type})";
                }
            }

            return $"{keyFrame.Value}@{keyFrame.Frame}({keyFrame.Easing.Type})";
        }

        XElement FromPath(Path content)
        {
            return new XElement("Path", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        XElement FromEllipse(Ellipse content)
        {
            return new XElement("Ellipse", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable(nameof(content.Diameter), content.Diameter);
                yield return FromAnimatable(nameof(content.Position), content.Position);
            }
        }

        XElement FromShape(Path content)
        {
            return new XElement("Path", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        XElement FromRectangle(Rectangle content)
        {
            return new XElement("Rectangle", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable(nameof(content.Size), content.Size);
                yield return FromAnimatable(nameof(content.Position), content.Position);
                yield return FromAnimatable(nameof(content.CornerRadius), content.CornerRadius);
            }
        }

        XElement FromPolystar(Polystar content)
        {
            return new XElement("Polystar", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        XElement FromTrimPath(TrimPath content)
        {
            return new XElement("TrimPath", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable(nameof(content.StartTrim), content.StartTrim);
                yield return FromAnimatable(nameof(content.EndTrim), content.EndTrim);
                yield return FromAnimatable(nameof(content.Offset), content.Offset);
            }
        }

        XElement FromMarker(Marker obj)
        {
            return new XElement(nameof(Marker), GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetLottieObjectContents(obj))
                {
                    yield return item;
                }

                yield return new XAttribute(nameof(obj.Frame), obj.Frame);
                yield return new XAttribute(nameof(obj.DurationMilliseconds), obj.DurationMilliseconds);
            }
        }

        XElement FromMergePaths(MergePaths content)
        {
            return new XElement("MergePaths", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        XElement FromRoundedCorner(RoundedCorner content)
        {
            return new XElement("RoundedCorner", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }

                yield return FromAnimatable(nameof(content.Radius), content.Radius);
            }
        }

        XElement FromRepeater(Repeater content)
        {
            return new XElement("Repeater", GetContents());
            IEnumerable<XObject> GetContents()
            {
                foreach (var item in GetShapeLayerContentContents(content))
                {
                    yield return item;
                }
            }
        }

        IEnumerable<XObject> GetShapeLayerContentContents(ShapeLayerContent content)
        {
            foreach (var item in GetLottieObjectContents(content))
            {
                yield return item;
            }

            if (!string.IsNullOrWhiteSpace(content.MatchName))
            {
                yield return new XAttribute(nameof(content.MatchName), content.MatchName);
            }
        }

        IEnumerable<XObject> GetLottieObjectContents(LottieObject obj)
        {
            if (!string.IsNullOrWhiteSpace(obj.Name))
            {
                yield return new XAttribute(nameof(obj.Name), obj.Name);
            }
        }

        XElement Unknown() => new XElement("UnknownElement");

        // The code we hit is supposed to be unreachable. This indicates a bug.
        static Exception Unreachable => new InvalidOperationException("Unreachable code executed");
    }
}
