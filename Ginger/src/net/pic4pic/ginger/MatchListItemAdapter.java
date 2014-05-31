package net.pic4pic.ginger;

import java.util.ArrayList;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.GingerHelpers;

public class MatchListItemAdapter extends ArrayAdapter<MatchedCandidate> {
	
	private Context context;
	private ArrayList<MatchedCandidate> people;
	
	private class ViewCache{
		public ImageView avatarImageView;
	    public TextView usernameTextView;
	    public TextView shortBioTextView;
	}
	
	public MatchListItemAdapter(Context context, ArrayList<MatchedCandidate> values) {	
	    super(context, R.layout.match_list_item, values);
	    this.context = context;
	    this.people = values;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {		
		
		View rowView = convertView;
		if (rowView == null) {
			LayoutInflater inflater = LayoutInflater.from(this.context);
			rowView = inflater.inflate(R.layout.match_list_item, null);
			ViewCache viewCache = new ViewCache();
			viewCache.avatarImageView = (ImageView) rowView.findViewById(R.id.avatar);
			viewCache.usernameTextView = (TextView) rowView.findViewById(R.id.username);
			viewCache.shortBioTextView = (TextView) rowView.findViewById(R.id.shortBio);	      
			rowView.setTag(viewCache);
		}
	
		ViewCache cachedView = (ViewCache) rowView.getTag();
		
		MatchedCandidate person = this.people.get(position);
		
		cachedView.usernameTextView.setText(person.getCandidateProfile().getUsername());
		cachedView.shortBioTextView.setText(person.getCandidateProfile().getShortBio());
		
		// set type face: bold vs. regular
		/*
		if(person.isViewed()){
			cachedView.usernameTextView.setTypeface(null, Typeface.NORMAL);
			cachedView.shortBioTextView.setTypeface(null, Typeface.NORMAL);
		}
		else{
			cachedView.usernameTextView.setTypeface(null, Typeface.BOLD);
			cachedView.shortBioTextView.setTypeface(null, Typeface.BOLD);
		}
		*/
		
		// set background color.
		rowView.setBackground(GingerHelpers.getListItemBackgroundDrawable(this.context, person.isViewed()));
		
		// set the default image
		cachedView.avatarImageView.setImageResource(android.R.drawable.ic_menu_gallery);
		
		// set the real image with an asynchronous download operation
		ImageFile imageToDownload = person.getProfilePics().getThumbnail();
		ImageDownloadTask asyncTask = new ImageDownloadTask(imageToDownload.getId(), cachedView.avatarImageView);
		asyncTask.execute(imageToDownload.getCloudUrl());
		
		return rowView;
	}	
}
