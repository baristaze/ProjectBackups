package net.pic4pic.ginger;

import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.PageAdvancer;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Html;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class PhotoInfoFragment extends Fragment {

	// constructor cannot have any parameters!!!
	public PhotoInfoFragment(/*no parameter here please*/){
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.fragment_photo_info, container, false);
		this._applyData(rootView);
		Button galleryButton = (Button)rootView.findViewById(R.id.galleryButton);
		galleryButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {	
				
				MyLog.bag()
				.add("funnel", "signup")
				.add("step", "3")
				.add("page", "photoinfo")
				.add("action", "click gallery")
				.m();
				
				((PageAdvancer)PhotoInfoFragment.this.getActivity()).moveToNextPage(0);
			}});
		
		Button cameraButton = (Button)rootView.findViewById(R.id.cameraButton);
		cameraButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				
				MyLog.bag()
				.add("funnel", "signup")
				.add("step", "3")
				.add("page", "photoinfo")
				.add("action", "click camera")
				.m();
				
				((PageAdvancer)PhotoInfoFragment.this.getActivity()).moveToNextPage(1);
			}});
		
		return rootView;
	}
	
	private void _applyData(View rootView){
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
				getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		String username = prefs.getString(this.getString(R.string.pref_username_key), "");		
		String info = String.format(this.getInfo(), username);
		
		TextView infoTextView = (TextView)(rootView.findViewById(R.id.infoTextView));
		infoTextView.setText(Html.fromHtml(info));	
	}
	
	public void applyData(){
		View rootView = this.getView();
		if(rootView != null){
			this._applyData(rootView);
		}
		else{
			MyLog.bag().e("applyData() has been called before onCreateView() of PhotoInfoFragment");
		}
	}
	
	private String getInfo(){
		StringBuilder s = new StringBuilder();
		s.append("Hi <strong><font color=\"#FF4444\">%s</font></strong>,<br/>");
		s.append("<br/>");
		s.append("This app is all about privacy and verified info for a safe and discreet dating. We value your privacy a lot. Nobody will see your private info (e.g. your photo) unless following <u>two conditions</u> are met.<br/>");
		s.append("<br/>");
		s.append("<strong>1)</strong> You explicitly select a person and send him/her a <strong>pic4pic</strong> request<br/>");
		s.append("<br/>");
		s.append("<strong>2)</strong> The person provides a valid face picture and accept your <strong>pic4pic</strong> request<br/>");
		s.append("<br/>");
		s.append("After these two conditions are met, we unlock the pictures for both sides at the same time. Isn't that neat?<br/>");
		s.append("<br/>");
		s.append("Now, we need to make sure that you have a valid face picture. This photo will be <strong>private</strong>. You can use it for your pic4pic requests.");
		
        return s.toString();
	} 
}
