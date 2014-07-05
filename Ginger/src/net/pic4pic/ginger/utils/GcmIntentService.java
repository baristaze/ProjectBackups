package net.pic4pic.ginger.utils;

import net.pic4pic.ginger.LaunchActivity;
import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.PushNotification;

import com.google.android.gms.gcm.GoogleCloudMessaging;

import android.app.IntentService;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.NotificationCompat;

public class GcmIntentService extends IntentService {
	
    public static final int NOTIFICATION_ID = 1;
    
    public GcmIntentService() {
        super("GcmIntentService");
    }

    @Override
    protected void onHandleIntent(Intent intent) {
    	
        Bundle extras = intent.getExtras();
        GoogleCloudMessaging gcm = GoogleCloudMessaging.getInstance(this);
        
        // The getMessageType() intent parameter must be the intent you received
        // in your BroadcastReceiver.
        String messageType = gcm.getMessageType(intent);
        
        if (!extras.isEmpty()) {
            /*
             * Filter messages based on message type. Since it is likely that GCM
             * will be extended in the future with new message types, just ignore
             * any message types you're not interested in, or that you don't
             * recognize.
             */
            if (GoogleCloudMessaging.MESSAGE_TYPE_SEND_ERROR.equals(messageType)) {
            	MyLog.e("GcmIntentService", "Send error: " + extras.toString());
            } 
            else if (GoogleCloudMessaging.MESSAGE_TYPE_DELETED.equals(messageType)) {
            	MyLog.e("GcmIntentService", "Deleted messages on server: " + extras.toString());   
            } 
            else if (GoogleCloudMessaging.MESSAGE_TYPE_MESSAGE.equals(messageType)) {
                // Post notification of received message.
                MyLog.i("GcmIntentService", "Received: " + extras.toString());
                sendNotification(extras);
            }
        }
        
        // Release the wake lock provided by the WakefulBroadcastReceiver.
        GcmBroadcastReceiver.completeWakefulIntent(intent);
    }

    // Put the message into a notification and post it.
    // This is just one simple example of what you might choose to do with
    // a GCM message.
    private void sendNotification(Bundle extras) {
    	
    	NotificationManager notificationManager = (NotificationManager)this.getSystemService(Context.NOTIFICATION_SERVICE);
        
        PendingIntent contentIntent = PendingIntent.getActivity(this, 0, new Intent(this, LaunchActivity.class), 0);

        PushNotification notification = PushNotification.createDefault();
        		
        String jsonData = extras.getString("Pic4PicJsonData", "");
        if(jsonData != null && !jsonData.isEmpty()){
        	try{
        		notification = GingerNetUtils.createFromJsonString(jsonData, PushNotification.class);
        	}
        	catch(Exception e){
        		MyLog.e("GcmIntentService", "Notification data couldn't be converted to object: " + jsonData);
        	}
        }
        else{
        	MyLog.e("GcmIntentService", "Retrieved push notification data is null or empty: " + extras.toString());
        }
        
        
        int iconId = R.drawable.ic_stb_envelope;
        if(notification.getSmallIcon() == 2){
        	iconId = R.drawable.ic_stb_favourites;
        }
        
        NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
        .setSmallIcon(iconId)
        .setContentTitle(notification.getTitle())
        .setStyle(new NotificationCompat.BigTextStyle()
        .bigText(notification.getMessage()))
        .setContentText(notification.getMessage());

        builder.setContentIntent(contentIntent);
        notificationManager.notify(NOTIFICATION_ID, builder.build());
    }
}