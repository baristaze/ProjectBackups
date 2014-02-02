package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.List;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ExpandableListView;
import android.widget.ListView;

public class NotificationListFragment extends Fragment {

	// a public empty constructor is a must in fragment. 
	// Do not add any parameter to this constructor.
	public NotificationListFragment() {
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.notif_listview, container, false);		
		final ExpandableListView listview = (ExpandableListView) rootView.findViewById(R.id.notifList);		
		listview.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
		
		ArrayList<Notification> list = this.getNotificationList();	    
		final NotificationListItemAdapter adapter = new NotificationListItemAdapter(this.getActivity(), list);
		listview.setAdapter(adapter);
		
		return rootView;
	}
	
	private ArrayList<Notification> getNotificationList(){
		
		ArrayList<Notification> list = new ArrayList<Notification>();
		
		// 1
		Person p = new Person();
	    p.setUsername("Jennifer123");
	    p.setShortBio("23 / F / Single / Seattle / Teacher");
	    p.setAvatarUri("http://wcdn3.dataknet.com/static/resources/icons/set47/790d2343.png");
	    p.setMainPhoto("http://upload.wikimedia.org/wikipedia/commons/0/0a/Robert_Downey_Jr_avp_Iron_Man_3_Paris.jpg");
	    p.setDescription("Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo.");
	    p.setGender(Gender.Female);
	    p.setFamiliarity(Familiarity.Familiar);
	    
	    Notification n = new Notification();
	    n.setTitle("Accepted your pic4pic");
	    n.setType(NotificationType.AcceptedP4P);
	    n.setRecommendedAction(NotificationAction.ViewProfile);
	    n.setSender(p);
	    n.setSentTime("53 mins ago");
	    list.add(n);
	    
	    // 2
	    p = new Person();
	    p.setUsername("Lora79 The Princes");
	    p.setShortBio("34 / F / Married / Kirkland / Lawyer / Lorem ipsum dolor sit amet.");
	    p.setAvatarUri("http://www.designdownloader.com/item/pngl/user_f020/user_f020-20111112123715-00005.png");
	    //p.setMainPhoto("http://themify.me/demo/themes/pinshop/files/2012/12/man-in-suit.jpg");
	    p.setMainPhoto("http://4.bp.blogspot.com/_dO5wi4i0JDs/TSYVpF2xp6I/AAAAAAAAAns/Khd0ETSNNvA/s1600/ad.jpg");
	    p.setDescription("Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo.");
	    p.setFamiliarity(Familiarity.Familiar);
	    p.setGender(Gender.Female);
	    
	    /* don't include the message count as in 'Sent you a message (3)'. It doesn't fit */
	    n = new Notification();
	    n.setTitle("Sent you a message");
	    n.setType(NotificationType.SentText);
	    n.setRecommendedAction(NotificationAction.ViewMessage);
	    n.setSender(p);	
	    n.setSentTime("23 hours ago");
	    list.add(n);
	    
	    // 3
	    p = new Person();
	    p.setUsername("Sara1234");
	    p.setShortBio("25 / F / Single / Redmond / Software Engineer / Lorem ipsum dolor sit amet.");
	    p.setAvatarUri("http://www.designdownloader.com/item/pngl/user_f020/user_f020-20111112123715-00005.png");
	    p.setMainPhoto("http://upload.wikimedia.org/wikipedia/commons/0/0a/Robert_Downey_Jr_avp_Iron_Man_3_Paris.jpg");
	    p.setDescription("Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo.");
	    p.setGender(Gender.Female);
	    p.setFamiliarity(Familiarity.Stranger);
	    
	    n = new Notification();
	    n.setTitle("Liked your bio");
	    n.setType(NotificationType.LikedBio);
	    n.setRecommendedAction(NotificationAction.RequestP4P);
	    n.setSender(p);
	    n.setSentTime("3 days ago");
	    list.add(n);
	    
	    // 4
	    p = new Person();
	    p.setUsername("John");
	    p.setShortBio("38 / M / Single / Sammamish / Mechanical Engineer / Lorem ipsum dolor sit amet.");
	    p.setAvatarUri("http://www.designdownloader.com/item/pngl/user_f020/user_f020-20111112123715-00005.png");
	    p.setMainPhoto("http://themify.me/demo/themes/pinshop/files/2012/12/man-in-suit.jpg");
	    p.setDescription("Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo.");
	    p.setFamiliarity(Familiarity.Stranger);
	    p.setGender(Gender.Male);
	    
	    n = new Notification();
	    n.setTitle("Sent you a pic4pic");
	    n.setType(NotificationType.RequestingP4P);
	    n.setRecommendedAction(NotificationAction.AcceptP4P);
	    n.setSender(p);	    
	    list.add(n);
	    n.setSentTime("more than a week ago");
	    
	    // 5
 		p = new Person();
 	    p.setUsername("Ashley McDonalds 8899");
 	    p.setShortBio("38 / F / Single / Yakima / Nurse / Lorem ipsum dolor sit amet");
 	    p.setAvatarUri("http://wcdn3.dataknet.com/static/resources/icons/set47/790d2343.png");
 	    p.setMainPhoto("http://upload.wikimedia.org/wikipedia/commons/0/0a/Robert_Downey_Jr_avp_Iron_Man_3_Paris.jpg");
 	    p.setDescription("Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo.");
 	    // p.setGender(Gender.Female);
 	    p.setFamiliarity(Familiarity.Stranger);
 	   
 	    n = new Notification();
 	    n.setTitle("Viewed your profile");
 	    n.setType(NotificationType.ViewedProfile);
 	    n.setRecommendedAction(NotificationAction.RequestP4P);
 	    n.setSender(p);
 	    n.setSentTime("more than a week ago");
 	    list.add(n);
	    
 	    String commonUrl = "http://tvmedia.ign.com/tv/image/article/805/805797/bionic-woman-2007-20070717053021720.jpg"; 
 	    String commonUrl2 = "http://tvreviews.files.wordpress.com/2007/10/michelle-ryan-bionic-woman.jpg";
 	    for(int x=0; x<list.size(); x++){
 	    	p = list.get(x).getSender();
 	    	if(p.getFamiliarity() == Familiarity.Familiar){
 	    		List<ImageInfo> photos = p.getOtherPhotos();
 	 	    	for(int y=0; y<7; y++){
 	 	    		ImageInfo imgInfo = new ImageInfo();
 	 	 	    	imgInfo.setThumbnail((y%2 == 0) ? commonUrl : commonUrl2);
 	 	 	    	photos.add(imgInfo);	
 	 	    	}
 	    	}
 	    }
 	    
	    return list;
	}	
}
