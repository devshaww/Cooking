using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProgressible
{
	public event EventHandler<OnProgressChangeEventArgs> OnProgressUpdate;
    public class OnProgressChangeEventArgs : EventArgs {
		public float progressNormalized;
	}
}
