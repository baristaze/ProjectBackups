package net.pic4pic.ginger;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import net.pic4pic.ginger.entities.Familiarity;
import net.pic4pic.ginger.entities.Gender;
import net.pic4pic.ginger.entities.Person;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.ImageClickListener;
import net.pic4pic.ginger.utils.ImageGalleryView;

public class PersonActivity extends Activity {

	public static final String PersonType = "net.pic4pic.ginger.Person"; 
	public static final int PersonActivityCode = 201;
	
	private Person person;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_person);
		
		Intent intent = getIntent();
		this.person = (Person)intent.getSerializableExtra(PersonType);
		this.setTitle(this.person.getUsername());
		
		// Show the Up button in the action bar.
		setupActionBar();
		
		this.fillData();
	}
	
	private void fillData(){
		TextView usernameText = (TextView)this.findViewById(R.id.candidateUsername);
		usernameText.setText(this.person.getUsername());
		
		TextView shortBioText = (TextView)this.findViewById(R.id.candidateShortBio);
		shortBioText.setText(this.person.getShortBio());
		
		String descr = this.person.getDescription();
		TextView descrText = (TextView)this.findViewById(R.id.candidateDescription);
		if(descr == null || descr.trim().length() <= 0){
			descrText.setVisibility(View.GONE);
		}
		else{
			descrText.setText(descr);
		}		
		
		ImageView avatarView = (ImageView)this.findViewById(R.id.candidateAvatar);
		avatarView.setImageResource(android.R.drawable.ic_menu_gallery);
		ImageDownloadTask avatarDownloadTask = new ImageDownloadTask(avatarView);
		avatarDownloadTask.execute(person.getAvatarUri());
		
		ImageView mainPhotoView = (ImageView)this.findViewById(R.id.candidateMainPhoto);
		mainPhotoView.setImageResource(android.R.drawable.ic_menu_gallery);
		ImageDownloadTask mainPhotoDownloadTask = new ImageDownloadTask(mainPhotoView, true);
		mainPhotoDownloadTask.execute(person.getMainPhoto());
		
		this.showHideActionButtons();
		
		if(person.getFamiliarity() == Familiarity.Familiar){
			mainPhotoView.setOnClickListener(new ImageClickListener(this, R.id.candidateMainPhoto));			
		}
		else{					
			this.addWatermark();
		}
		
		LinearLayout photoGalleryParent = (LinearLayout)this.findViewById(R.id.candidateView);
		ImageGalleryView gallery = new ImageGalleryView(this, photoGalleryParent, this.person.getOtherPhotos()); 
		gallery.fillPhotos();
	}
	
	private void showHideActionButtons(){
		
		if(this.person.getFamiliarity() == Familiarity.Familiar){
			LinearLayout buttonGroup = (LinearLayout)this.findViewById(R.id.candidateAnonymousActions);
			// buttonGroup.setVisibility(View.INVISIBLE);
			buttonGroup.setVisibility(View.GONE);
		}
		else{
			LinearLayout buttonGroup = (LinearLayout)this.findViewById(R.id.candidateActions);
			buttonGroup.setVisibility(View.GONE);
		}
	}

	private void addWatermark(){
		String readyText = this.getString(R.string.candidate_ready_for_p4p);
		if(this.person.getGender() == Gender.Female){
			readyText = this.getString(R.string.candidate_ready_for_p4p_she);
		}
		else if(this.person.getGender() == Gender.Male){
			readyText = this.getString(R.string.candidate_ready_for_p4p_he);
		}
		
		View watermarkView = this.getLayoutInflater().inflate(R.layout.watermark, null);
		TextView watermark = (TextView)watermarkView.findViewById(R.id.watermarkText);
		watermark.setText(readyText);
		
		FrameLayout imageContainer = (FrameLayout)this.findViewById(R.id.candidateMainPhotoContainer);
		imageContainer.addView(watermark);
	}
	
	/**
	 * Set up the {@link android.app.ActionBar}.
	 */
	private void setupActionBar() {
		getActionBar().setDisplayHomeAsUpEnabled(true);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.person, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			//NavUtils.navigateUpFromSameTask(this);
			finish();
			return true;
		}
		return super.onOptionsItemSelected(item);
	}
}
