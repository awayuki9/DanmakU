using UnityEngine;
using System.Collections.Generic;
using UnityUtilLib;

namespace Danmaku2D {
	
	public class SourcePoint {
		public Vector2 Position;
		public float BaseRotation;

		public SourcePoint(Vector2 location, float rotation) {
			this.Position = location;
			this.BaseRotation = rotation;
		}
	}

	public abstract class ProjectileSource : CachedObject {

		public ProjectileSource subSource;

		[System.NonSerialized]
		public DanmakuField TargetField;

		protected List<SourcePoint> sourcePoints;

		protected void UpdatePoints(Vector2 position, float rotation) {
			UpdateSourcePoints(position, rotation);
			float sourceRotation;
			if (subSource != null) {
				List<SourcePoint> temp = new List<SourcePoint> ();
				for (int i = 0; i < sourcePoints.Count; i++) {
					if (subSource.sourcePoints == null) {
						subSource.sourcePoints = new List<SourcePoint> ();
					}
					sourceRotation = sourcePoints [i].BaseRotation;
					subSource.UpdatePoints (sourcePoints [i].Position, sourceRotation);
					for (int j = 0; j < subSource.sourcePoints.Count; j++) {
						SourcePoint point = subSource.sourcePoints [j];
						//point.BaseRotation += rotation;
						temp.Add (point);
					}
				}
				sourcePoints = temp;
			}
		}

		protected abstract void UpdateSourcePoints (Vector2 position, float rotation);

		void Update() {
			if (transform.hasChanged) {
				UpdatePoints(transform.position, transform.rotation.eulerAngles.z);
				transform.hasChanged = false;
			}
		}

		public override void Awake () {
			base.Awake ();
			TargetField = Util.FindClosest<DanmakuField> (transform.position);
			sourcePoints = new List<SourcePoint> ();
			UpdatePoints(transform.position, transform.rotation.eulerAngles.z);
		}

		public SourcePoint[] SourcePoints {
			get {
				UpdateSourcePoints(transform.position, transform.eulerAngles.z);
				return sourcePoints.ToArray();
			}
		}

		public void Fire(ProjectilePrefab prefab,
		                 float velocity,
		                 float rotationOffset = 0,
		                 float angularVelocity = 0,
		                 ProjectileController controller = null,
		                 FireModifier modifier = null) {
			if(TargetField == null) {
				Debug.LogWarning("Firing from a Projectile Source without a Target Field");
				return;
			}
			for (int i = 0; i < sourcePoints.Count; i++) {
				SourcePoint source = sourcePoints[i];
				TargetField.FireCurved (prefab,
                                        source.Position,
                                        source.BaseRotation + rotationOffset,
				                        velocity,
				                        angularVelocity,
				                        DanmakuField.CoordinateSystem.World,
                                        controller);
			}
		}

		public void Fire(FireBuilder data) {
			if(TargetField == null) {
				Debug.LogWarning("Firing from a Projectile Source without a Target Field");
				return;
			}
			ProjectileController controller = data.Controller;
			FireBuilder copy = data.Clone ();
			float rotationOffset = data.Rotation;
			copy.CoordinateSystem = DanmakuField.CoordinateSystem.World;
			copy.Controller = controller;
			for (int i = 0; i < sourcePoints.Count; i++) {
				SourcePoint source = sourcePoints[i];
				copy.Position = source.Position;
				copy.Rotation = source.BaseRotation + rotationOffset;
				TargetField.Fire(copy);
			}
		}

		void OnDrawGizmos() {
			if (sourcePoints == null) { 
				sourcePoints = new List<SourcePoint> ();
			}
			UpdatePoints(transform.position, transform.rotation.eulerAngles.z);
			for(int i = 0; i < sourcePoints.Count; i++) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(sourcePoints[i].Position, 1f);
				Gizmos.color = Color.red;
				Vector3 endRay = sourcePoints[i].Position + 5 * Util.OnUnitCircle(sourcePoints[i].BaseRotation + 90f).normalized;
				Gizmos.DrawLine(sourcePoints[i].Position, endRay);
			}
		}
	}
	
}