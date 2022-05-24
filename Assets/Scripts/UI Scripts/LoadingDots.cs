using System.Collections.Generic;
using UnityEngine;

public class LoadingDots : MonoBehaviour {
    public float repeatTime = 1;
    public float bounceTime = 0.25f;
    public float bounceHeight = 10;
    public Transform[] dots;

    private void Update() {
        for (int i = 0; i < this.dots.Length; i++) {
            var p = this.dots[i].localPosition;
            var t = Time.time * (1/this.repeatTime) * Mathf.PI + p.x;
            var y = (Mathf.Cos(t) - this.bounceTime) / (1f - this.bounceTime);
            p.y = Mathf.Max(0, y * this.bounceHeight);
            this.dots[i].localPosition = p;
        }
    }
}
