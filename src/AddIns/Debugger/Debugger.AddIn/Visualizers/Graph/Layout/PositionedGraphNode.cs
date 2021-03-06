﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ObjectNode with added position information.
	/// </summary>
	public class PositionedGraphNode : SplineRouting.IRect
	{
		public static readonly double MaxHeight = 300;
		
		/// <summary>
		/// Creates new PositionedNode.
		/// </summary>
		/// <param name="objectNode">Underlying ObjectNode.</param>
		public PositionedGraphNode(ObjectGraphNode objectNode)
		{
			this.objectNode = objectNode;
			initVisualControl();
		}
		
		public event EventHandler<PositionedPropertyEventArgs> PropertyExpanded;
		public event EventHandler<PositionedPropertyEventArgs> PropertyCollapsed;
		public event EventHandler<ContentNodeEventArgs> ContentNodeExpanded;
		public event EventHandler<ContentNodeEventArgs> ContentNodeCollapsed;
		
		private ObjectGraphNode objectNode;
		/// <summary>
		/// Underlying ObjectNode.
		/// </summary>
		public ObjectGraphNode ObjectNode
		{
			get { return objectNode; }
		}
		
		/// <summary>
		/// Tree-of-properties content of this node.
		/// </summary>
		public ContentNode Content
		{
			get; set;
		}
		
		private PositionedGraphNodeControl nodeVisualControl;
		/// <summary>
		/// Visual control to be shown for this node.
		/// </summary>
		public PositionedGraphNodeControl NodeVisualControl
		{
			get
			{
				return this.nodeVisualControl;
			}
		}
		
		// TODO for speedup of re-layout only, posNodeForObjectGraphNode will be a service, that will return existing posNodes or create empty new
		public void InitContentFromObjectNode(Expanded expanded)
		{
			this.Content = new ContentNode(this, null);
			this.Content.InitFrom(this.ObjectNode.Content, expanded);
			this.nodeVisualControl.Root = this.Content;
		}
		
		private void initVisualControl()
		{
			this.nodeVisualControl = NodeControlCache.Instance.GetNodeControl();
			this.nodeVisualControl.MaxHeight = MaxHeight;
			
			// propagate events from nodeVisualControl
			this.nodeVisualControl.PropertyExpanded += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_PropertyExpanded);
			this.nodeVisualControl.PropertyCollapsed += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_PropertyCollapsed);
			this.nodeVisualControl.ContentNodeExpanded += new EventHandler<ContentNodeEventArgs>(NodeVisualControl_ContentNodeExpanded);
			this.nodeVisualControl.ContentNodeCollapsed += new EventHandler<ContentNodeEventArgs>(NodeVisualControl_ContentNodeCollapsed);
		}
		
		public IEnumerable<PositionedNodeProperty> Properties
		{
			get
			{
				return this.Content.FlattenProperties();
			}
		}
		
		public virtual IEnumerable<PositionedEdge> Edges
		{
			get
			{
				foreach	(PositionedNodeProperty property in this.Properties)
				{
					if (property.Edge != null)
						yield return property.Edge;
				}
			}
		}
		
		public void Measure()
		{
			this.nodeVisualControl.Measure(new Size(800, 800));
		}
		
		public double Left { get; set; }
		public double Top { get; set; }
		public double Width
		{
			get { return NodeVisualControl.DesiredSize.Width; }
		}
		public double Height
		{
			get { return NodeVisualControl.DesiredSize.Height; }
		}
		
		public Point LeftTop
		{
			get { return new Point(Left, Top); }
		}
		
		public Point Center
		{
			get { return new Point(Left + Width / 2, Top + Height / 2); }
		}
		
		public Rect Rect { get { return new Rect(Left, Top, Width, Height); } }
		
		#region event helpers
		private void NodeVisualControl_PropertyExpanded(object sender, PositionedPropertyEventArgs e)
		{
			if (this.PropertyExpanded != null)
				this.PropertyExpanded(sender, e);
		}
		
		private void NodeVisualControl_PropertyCollapsed(object sender, PositionedPropertyEventArgs e)
		{
			if (this.PropertyCollapsed != null)
				this.PropertyCollapsed(sender, e);
		}
		
		private void NodeVisualControl_ContentNodeExpanded(object sender, ContentNodeEventArgs e)
		{
			if (this.ContentNodeExpanded != null)
				this.ContentNodeExpanded(sender, e);
		}
		
		private void NodeVisualControl_ContentNodeCollapsed(object sender, ContentNodeEventArgs e)
		{
			if (this.ContentNodeCollapsed != null)
				this.ContentNodeCollapsed(sender, e);
		}
		#endregion
		
		SplineRouting.Box box;
		public SplineRouting.IRect Inflated(double padding)
		{
			if (box == null)
				box = new SplineRouting.Box(this);
			return box.Inflated(padding);
		}
	}
}
