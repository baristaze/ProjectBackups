package net.pic4pic.ginger;

import java.util.ArrayList;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;

import android.widget.AbsListView.LayoutParams;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

import net.pic4pic.ginger.entities.Gender;
import net.pic4pic.ginger.entities.Person;

public class MatchListFragment extends Fragment {
	
	// a public empty constructor is a must in fragment. 
	// Do not add any parameter to this constructor.
	public MatchListFragment(/*no parameter here please*/) {
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.match_listview, container, false);		
		final ListView listview = (ListView) rootView.findViewById(R.id.matchList);		
		View footer = inflater.inflate(R.layout.show_more_btn, null);		
		float height = this.getActivity().getResources().getDimension(R.dimen.person_li_footer_height);
		LayoutParams lp = new LayoutParams(LayoutParams.MATCH_PARENT, (int) height);		
		footer.setLayoutParams(lp);
		listview.addFooterView(footer);
		//listview.addFooterView(footer, null, false);
		//listview.setDivider(null);
		//listview.setDividerHeight(0);
	    
		Button showMoreButton = (Button)footer.findViewById(R.id.showMoreButton);
		showMoreButton.setOnClickListener(new OnClickListener() {
			@Override
		    public void onClick(View v) {
		    	onShowMoreMatches(v);
		    }
		 });
		
		ArrayList<Person> plist = this.getMatchList();	    
		final MatchListItemAdapter adapter = new MatchListItemAdapter(this.getActivity(), plist);
		listview.setAdapter(adapter);
		
		listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, final View view, int position, long id) {
				Person item = (Person) parent.getItemAtPosition(position);
				onShowPersonDetails(item, view);
			}
		});
		
		return rootView;
	}
	
	public void onShowMoreMatches(View v){
		Toast.makeText(this.getActivity(), "Please purchase some credit", Toast.LENGTH_LONG).show();
	}
	
	public void onShowPersonDetails(Person person, View v){
		// Toast.makeText(this.getActivity(), "Showing " + person, Toast.LENGTH_LONG).show();
		Intent intent = new Intent(this.getActivity(), PersonActivity.class);
		intent.putExtra(PersonActivity.PersonType, person);

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed.
		startActivityForResult(intent, PersonActivity.PersonActivityCode);
	}
	
	private ArrayList<Person> getMatchList(){
		
		ArrayList<Person> plist = new ArrayList<Person>();
		
		Person p = new Person();
	    p.setUsername("Jennifer123");
	    p.setShortBio("23 / F / Single / Seattle / Teacher");
	    p.setAvatarUri("http://wcdn3.dataknet.com/static/resources/icons/set47/790d2343.png");
	    p.setMainPhoto("http://upload.wikimedia.org/wikipedia/commons/0/0a/Robert_Downey_Jr_avp_Iron_Man_3_Paris.jpg");
	    p.setDescription("Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo. Persius officiis eloquentiam ut sed,ius nostrud sensibus ea.");
	    p.setGender(Gender.Female);
	    plist.add(p);	    
	    
	    p = new Person();
	    p.setUsername("Lora79 The Princes");
	    p.setShortBio("34 / F / Married / Kirkland / Lawyer / Lorem ipsum dolor sit amet.");
	    p.setAvatarUri("http://www.designdownloader.com/item/pngl/user_f020/user_f020-20111112123715-00005.png");
	    p.setMainPhoto("http://themify.me/demo/themes/pinshop/files/2012/12/man-in-suit.jpg");
	    p.setDescription("Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo. Persius officiis eloquentiam ut sed,ius nostrud sensibus ea.");
	    p.setGender(Gender.Female);
	    plist.add(p);
	    
	    for(int x=0; x<10; x++){
	    	if(x % 2 == 0){
	    		plist.add(plist.get(0));
	    	}else{
	    		plist.add(plist.get(1));
	    	}
	    }
	    
	    for(int x=0; x<5; x++){
	    	plist.add(plist.get(0));
	    }
	    
	    for(int x=0; x<5; x++){
	    	plist.add(plist.get(1));
	    }
	    
	    return plist;
	}
}
