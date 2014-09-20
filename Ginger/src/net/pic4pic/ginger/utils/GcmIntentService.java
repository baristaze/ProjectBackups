package net.pic4pic.ginger.utils;

import net.pic4pic.ginger.LaunchActivity;
import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.PushNotification;

import com.google.android.gms.gcm.GoogleCloudMessaging;

import android.app.IntentService;
import android.app.Notification;
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
            	MyLog.bag().e("Send error: " + extras.toString());
            } 
            else if (GoogleCloudMessaging.MESSAGE_TYPE_DELETED.equals(messageType)) {
            	MyLog.bag().e("Deleted messages on server: " + extras.toString());   
            } 
            else if (GoogleCloudMessaging.MESSAGE_TYPE_MESSAGE.equals(messageType)) {
                // Post notification of received message.
                MyLog.bag().i("Received: " + extras.toString());
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
    	
    	PushNotification notification = PushNotification.createDefault();        		
        String jsonData = extras.getString("Pic4PicJsonData", "");
        if(jsonData != null && !jsonData.isEmpty()){
        	try{
        		notification = GingerNetUtils.createFromJsonString(jsonData, PushNotification.class);
        	}
        	catch(Throwable e){
        		MyLog.bag().e("Notification data couldn't be converted to object: " + jsonData);
        	}
        }
        else{
        	MyLog.bag().e("Retrieved push notification data is null or empty: " + extras.toString());
        }
    	
        int iconId = R.drawable.ic_notif_2;
        int selectedTabIndex = 1; // 2nd tab
        if(notification.getActionType() == 88){
        	selectedTabIndex = 0;
        }
        
        NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
        .setSmallIcon(iconId)
        .setContentTitle(notification.getTitle())
        .setStyle(new NotificationCompat.BigTextStyle().bigText(notification.getMessage()))
        .setContentText(notification.getMessage())
        .setAutoCancel(true);

        Intent coreIntent = new Intent(this, LaunchActivity.class);
    	coreIntent.putExtra("PreSelectedTabIndexOnMainActivity", selectedTabIndex);    	
        PendingIntent contentIntent = PendingIntent.getActivity(this, 0, coreIntent, PendingIntent.FLAG_CANCEL_CURRENT);
        builder.setContentIntent(contentIntent);
        
        Notification note = builder.build();
        note.defaults |= Notification.DEFAULT_VIBRATE;
        note.defaults |= Notification.DEFAULT_SOUND;
        note.defaults |= Notification.DEFAULT_LIGHTS;
        
        NotificationManager notificationManager = (NotificationManager)this.getSystemService(Context.NOTIFICATION_SERVICE);
        notificationManager.notify(NOTIFICATION_ID, note);
    }
}