// <copyright file="CircleDescriptor.cs" company="Destiny Wireless">
// Copyright (c) 2017 All Rights Reserved
// <author>Jamie Duggan</author>
// </copyright>

namespace FormTools.FormDescriptor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Circle Descriptor
    /// </summary>
    public class CircleDescriptor : ShapeDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircleDescriptor"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="zOrder">The z order.</param>
        public CircleDescriptor(XElement element, int zOrder) : base(element, zOrder)
        {
            var centre = new Point { X = 0, Y = 0 };
            bool fillOpacityZero = false;
            foreach (var att in element.Attributes().ToList())
            {
                switch (att.Name.ToString())
                {
                    case FILL:
                        if (!fillOpacityZero)
                        {
                            FillColour = ColourFromString(att.Value, WhiteColour);
                        }
                        break;
                    case FILLOPACITY:
                        if(att.Value == "0")
                        {
                            FillColour = ColourFromString("transparent", WhiteColour);
                            fillOpacityZero = true;
                        }
                        break;
                    case CX:
                        if (!string.IsNullOrEmpty(att.Value))
                        {
                            centre.X = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        }
                        break;
                    case CY:
                        if (!string.IsNullOrEmpty(att.Value))
                        {
                            centre.Y = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        }
                        break;
                    case R:
                        if (!string.IsNullOrEmpty(att.Value))
                        {
                            Radius = double.Parse(att.Value, CultureInfo.InvariantCulture);
                        }
                        break;
                }
            }
            this.Centre = centre;
            Origin.X = this.Centre.X - Radius;
            Origin.Y = this.Centre.Y - Radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleDescriptor"/> class.
        /// </summary>
        /// <param name="original">The original.</param>
        public CircleDescriptor (CircleDescriptor original) : base(original)
        {
            FillColour = original.FillColour;
            this.Centre = original.Centre;
            Radius = original.Radius;
        }

        /// <summary>
        /// Center X attribute name
        /// </summary>
        private const string CX = "cx";

        /// <summary>
        /// Center Y attribute name
        /// </summary>
        private const string CY = "cy";

        /// <summary>
        /// Radius attribute name
        /// </summary>
        private const string R = "r";

        /// <summary>
        /// Gets or sets the centre.
        /// </summary>
        /// <value>
        /// The centre.
        /// </value>
        public Point Centre { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public double Radius { get; set; }

        #region Calculated Values
        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public new double X
        {
            get { return this.Centre.X - this.Radius; }
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public new double Y
        {
            get { return this.Centre.Y - this.Radius; }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public new double Width
        {
            get { return this.Radius * 2d; }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public new double Height
        {
            get { return this.Radius * 2d; }
        }
        #endregion
    }
}
