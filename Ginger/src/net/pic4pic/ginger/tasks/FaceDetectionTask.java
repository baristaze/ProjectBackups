package net.pic4pic.ginger.tasks;

import java.util.ArrayList;

import net.pic4pic.ginger.utils.MyLog;


import android.content.Context;
import android.graphics.Bitmap;
import android.media.FaceDetector;

public class FaceDetectionTask extends BlockedTask<String, Void, FaceDetector.Face[]> {
	
	private FaceDetectionListener listener;
	private Bitmap bitmap;
	private int maxFaceCount;
	
	public FaceDetectionTask(FaceDetectionListener listener, Context context, Bitmap bitmap, int maxFaceCount){			
		super(context, null, false);		
		this.listener = listener;
		this.bitmap = bitmap;
		this.maxFaceCount = maxFaceCount;
	}
		
    @Override
    protected FaceDetector.Face[] doInBackground(String... executeArgs) {
    	return detectFaces(this.bitmap, this.maxFaceCount);
    }
    
    private static FaceDetector.Face[] detectFaces(Bitmap bitmap, int maxFaceCount){
    	try{
    		long start = System.nanoTime();    		
    		ArrayList<FaceDetector.Face> all = new ArrayList<FaceDetector.Face>();
    		FaceDetector detector = new FaceDetector(bitmap.getWidth(), bitmap.getHeight(), maxFaceCount);
    		FaceDetector.Face[] buffer = new FaceDetector.Face[maxFaceCount];
			int detectCount = detector.findFaces(bitmap, buffer);
			for(int x=0; x<detectCount; x++){
				all.add(buffer[x]);
			}			
			long end = System.nanoTime();
			long millisecs = (end-start) / 1000000;
			if(millisecs < 1500){
				Thread.sleep(1500-millisecs);
			}
			
			return all.toArray(new FaceDetector.Face[detectCount]);
    	}
    	catch(Throwable ex){    
    		MyLog.bag().add(ex).e("Face detection failed");
    	}

    	return new FaceDetector.Face[0];
    }
    
    protected void onPostExecute(FaceDetector.Face[] detectedFaces) {    	
    	super.onPostExecute(detectedFaces);
    	this.listener.onDetect(this.bitmap, detectedFaces);
    }
    
    public interface FaceDetectionListener{    	
    	public void onDetect(Bitmap bitmap, FaceDetector.Face[] detectedFaces);
    }
}
