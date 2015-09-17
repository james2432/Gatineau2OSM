Gatineau2OSM
------------
<p>This project aims at providing an easy way to maintain Gatineau Address data from the Gatineau Website with OpenStreetMap.</p>

Functionality
------------
Converts the Gatineau CSV data format to a JOSM OSM format which can be viewed in JOSM and uploaded to OSM(data may already exist: see compare functionality).
Compare two datasets (CSV format) and will display the changes made between the two datasets.
*Experimental* Write changeset will write out the changeset to an osm file that can be used in JOSM. You *should* QA the data as it isn't perfect and you might have to delete data from the older dataset(might not exist in OSM anymore) to get it to work.
Note: The write changeset can be slow when dealing with 100+ changes that aren't adding nodes as it checks OSM via an XAPI to get versions and update the node/way



Datasets
------------
Archived datasets can be downloaded here:
http://tinyurl.com/p7myagw
New datasets can be downloaded on the gatineau website here:
http://gatineau.ca/donneesouvertes/fiche_metadonnees_en.aspx?id=1393013950