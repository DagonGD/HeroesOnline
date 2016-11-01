using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadMap : MonoBehaviour
{
    private const string ConsumerKey = "rr3PSu0EOYm1YdpR2kbRqIQzys3FyqwW";
    private const string MapType = "map";
    
    public Renderer mapRenderer;
    public int Zoom;
    public int MapSize;
    public Text txtCoords;
    public float distanceToUpdate;

    private Vector2? MapCoords = null;
    private int numberOfMapReuests = 0;

    void Start () {
        
	}

	void GpsUpdated(Vector2 coords)
    {
		txtCoords.text = string.Format ("X:{0}, Y:{1}, R:{2}", coords.x, coords.y, numberOfMapReuests);

        if(!MapCoords.HasValue || Vector2.Distance(MapCoords.Value, coords) > distanceToUpdate)
        {
            MapCoords = coords;
            numberOfMapReuests++;

            var url = string.Format("http://open.mapquestapi.com/staticmap/v4/getmap?key={0}&size={1},{1}&zoom={2}&type={3}&center={4},{5}",
            ConsumerKey, MapSize, Zoom, MapType, coords.x, coords.y);

            Debug.Log(url);

            StartCoroutine(LoadImage(url));
        }
    }

    void GpsError(string msg)
    {
        txtCoords.text = msg;
    }

    private IEnumerator LoadImage(string url)
    {
        var www = new WWW(url);

        while (!www.isDone)
        {
            yield return null;
        }

        if(www.error == null)
        {
            yield return new WaitForSeconds(0.5f);
            mapRenderer.material.mainTexture = null;
            var texture = new Texture2D(MapSize, MapSize, TextureFormat.RGB24, false);
            mapRenderer.material.mainTexture = texture;
            www.LoadImageIntoTexture(texture);
        }
        else
        {
            Debug.Log("Map Error: " + www.error);
            yield return new WaitForSeconds(1);
        }

        mapRenderer.enabled = true;

        SendMessage("MapUpdated");
    }
}
