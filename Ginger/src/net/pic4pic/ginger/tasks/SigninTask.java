package net.pic4pic.ginger.tasks;

import java.util.List;

import net.pic4pic.ginger.Familiarity;
import net.pic4pic.ginger.Gender;
import net.pic4pic.ginger.ImageInfo;
import net.pic4pic.ginger.LaunchActivity;
import net.pic4pic.ginger.Person;

import android.os.AsyncTask;

public class SigninTask extends AsyncTask<String, Void, Person> {
	
	private LaunchActivity activity;
	private String username;
	private String password;
	
		public SigninTask(LaunchActivity activity, String username, String password){
			this.activity = activity;
			this.username = username;
			this.password = password;
		}

	    @Override
	    protected Person doInBackground(String... executeArgs) {    	
	    	// make an HTTP post in a RESTfull way. Use JSON. 
	    	// Once you get the data, convert it to Person
	    	try {
				Thread.sleep(1500);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
	    	
	    	Person me = this.createTestData();
	        return me;
	    }

	    protected void onPostExecute(Person result) {
	    	this.activity.onSignin(result, this.username, this.password);
	    }
	    
		private Person createTestData(){
			
			String s = "Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo. Persius officiis eloquentiam ut sed,ius nostrud sensibus ea.";
			s += " " + s;
			
			Person p = new Person();
			p.setUsername(this.username);
			p.setAvatarUri("http://www.prosportstickers.com/product_images/h/curious_george_decal_head__26524.jpg");
			p.setShortBio("34 / M / Married / Redmond / Software Developer");
			p.setDescription(s);
			p.setFamiliarity(Familiarity.Familiar);
			p.setGender(Gender.Male);
			p.setMainPhoto("http://4.bp.blogspot.com/_dO5wi4i0JDs/TSYVpF2xp6I/AAAAAAAAAns/Khd0ETSNNvA/s1600/ad.jpg");
			
			String commonUrl = "http://tvmedia.ign.com/tv/image/article/805/805797/bionic-woman-2007-20070717053021720.jpg"; 
	 	    String commonUrl2 = "http://tvreviews.files.wordpress.com/2007/10/michelle-ryan-bionic-woman.jpg";
	 	     	    
	 	    List<ImageInfo> photos = p.getOtherPhotos();
	    	for(int y=0; y<11; y++){
	    		ImageInfo imgInfo = new ImageInfo();
	 	    	imgInfo.setThumbnail((y%2 == 0) ? commonUrl : commonUrl2);
	 	    	photos.add(imgInfo);	
	    	}
	    	 	    
	    	return p;
		}
}
