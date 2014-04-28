package net.pic4pic.ginger.utils;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.media.FaceDetector;
import android.util.Log;
import android.view.View;

public class DrawView extends View {

	private Bitmap bitmap;
	private FaceDetector.Face[] detectedFaces;

	public DrawView(Context context, Bitmap bitmap, FaceDetector.Face[] detectedFaces) {
		super(context);
		this.bitmap = bitmap;
		this.detectedFaces = detectedFaces;
		if (this.detectedFaces == null) {
			this.detectedFaces = new FaceDetector.Face[0];
		}
	}

	@Override
	public void onDraw(Canvas canvas) {
		Log.v("Ginger", "CustomDraw: onDraw is invoked");
		Log.v("Ginger", "CustomDraw: onDraw WxH = " + this.getWidth() + "x" + this.getHeight());
		Log.v("Ginger", "CustomDraw: onDraw WxH of Canvas = " + canvas.getWidth() + "x" + canvas.getHeight()); // same with parent

		if (this.bitmap != null) {
			Bitmap scaledBitmap = BitmapHelpers.scaleCenterCrop(this.bitmap,
					canvas.getWidth(), canvas.getHeight(), this.detectedFaces);

			canvas.drawBitmap(scaledBitmap, 0, 0, null);
		}
	}

	
	// below code is good if you want to use a certain size of the parent view; e.g. half of the height.
	/*  
	@SuppressLint("DrawAllocation")	  
	@Override protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec){ 
		Log.v("Ginger", "CustomDraw: onMeasure is invoked with WxH measure specs = " + widthMeasureSpec + "x" + heightMeasureSpec); 
		int parentWidth = MeasureSpec.getSize(widthMeasureSpec); 
		int parentHeight = MeasureSpec.getSize(heightMeasureSpec); 
		// make this width/2 if you want to have half-a-page 
		Log.v("Ginger", "CustomDraw: onMeasure Parent WxH = " + parentWidth + "x" + parentHeight); 
		this.setMeasuredDimension(parentWidth, parentHeight); 
		this.setLayoutParams(new FrameLayout.LayoutParams(parentWidth,parentHeight));
		super.onMeasure(widthMeasureSpec, heightMeasureSpec); 
	}
	*/	
}
