using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : Interactable
{
    public List<Item> ChestContent;                 //List of items inside chest

    public GameObject ChestContentUIPanel;          //Game object under which items from chest are displayed
    public GameObject PickUpPrompt;                 //Button for picking items up    

    public Image ChestContentIcon;                  //Image used for item representation inside chest

    public List<Item> DisplayedItems;               //Items that are currenty displayed on screen
    public List<Image> DisplayedItemsImages;        //Image components of items currently displayed on screen

    public bool ChestOpened = false;

    private List<Item> _stackedItems = new List<Item> ();
    private List<int> _stackNumbers = new List<int> ();

    private Animator _chestAC;
    private Animator _playerAC;

    public override void Start ( )
    {
        base.Start ();

        _chestAC = GetComponent<Animator> ();
        _playerAC = GameObject.Find("Player").GetComponent<Animator> ();

        int x = 1;

        for (int i = 0; i < ChestContent.Count - 1; i++)
        {
            if (ChestContent [i].Stackable && ChestContent [i + 1].Stackable)
            {
                if (ChestContent [i].Name == ChestContent [i + 1].Name)
                {
                    _stackedItems.Add (ChestContent [i]);
                    ChestContent.RemoveAt (i + 1);
                    i--;
                }
            }
        }

        for (int i = 0; i < _stackedItems.Count - 1; i++)
        {
            if (_stackedItems [i].Name == _stackedItems [i + 1].Name)
            {
                x++;
                _stackedItems.RemoveAt (i + 1);
                i--;
            }
            else
            {
                x++;
                _stackNumbers.Add (x);
                x = 1;
            }
        }

        x++;
        _stackNumbers.Add (x);
    }

    public override void Interact ( )
    {
        base.Interact ();

        if (!ChestOpened)
        {
            OpenChest ();
        }
        else if (!HasInteracted)
        {
            InteractPrompt.SetActive (false);
            StartCoroutine (ShowChestContent (0.0f));
            HasInteracted = true;
            PickUpPrompt.SetActive (true);
            PickUpPrompt.GetComponent<Button> ().Select ();

            if (ChestContent.Count != 0)
            {
                if (ChestContent.Count >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        DisplayedItems.Add (ChestContent [i]);
                        Image image = Instantiate (ChestContentIcon, ChestContentUIPanel.transform);
                        image.sprite = ChestContent [i].Icon;
                        image.name = ChestContent [i].Name;

                        DisplayedItemsImages.Add (image);

                        if (_stackedItems.Count != 0)
                        {
                            if (image.name == _stackedItems [0].Name)
                            {
                                image.GetComponentInChildren<Text> ().text = _stackNumbers [0].ToString ();
                                _stackedItems.RemoveAt (0);
                                _stackNumbers.RemoveAt (0);
                            }
                        }
                    }
                    ChestContent.RemoveRange (0, 3);
                }
                else
                {
                    for (int i = 0; i < ChestContent.Count; i++)
                    {
                        DisplayedItems.Add (ChestContent [i]);
                        Image image = Instantiate (ChestContentIcon, ChestContentUIPanel.transform);
                        image.sprite = ChestContent [i].Icon;
                        image.name = ChestContent [i].Name;

                        DisplayedItemsImages.Add (image);

                        if (_stackedItems.Count != 0)
                        {
                            int x = 0;
                            if (image.name == _stackedItems [x].Name)
                            {
                                image.GetComponentInChildren<Text> ().text = _stackNumbers [x].ToString ();
                                //_stackedItems.RemoveAt (0);
                                //_stackNumbers.RemoveAt (0);
                            }
                            x++;
                        }
                    }
                    ChestContent.Clear ();
                }
            }            
        }
        else
        {
            PickUpPrompt.SetActive (false);
            ChestContentUIPanel.SetActive (false);
        }
    }

    public void TakeItems ( )
    {
        //Add items to inventory
        for (int i = 0; i < DisplayedItems.Count; i++)
        {
            int x = 0;

            if (DisplayedItems [i].Stackable && _stackedItems.Count != 0)
            {
                if (DisplayedItems [i].Name == _stackedItems [x].Name)
                {
                    Inventory.instance.StackAmount = _stackNumbers [x];
                    x++;
                    Debug.Log ("Stacked item picked up");
                }
            }

            Inventory.instance.AddItem (DisplayedItems [i]);
            Inventory.instance.StackAmount = 1;            
        }         
        DisplayedItems.Clear ();

        //Clear displayed items
        foreach (Image image in DisplayedItemsImages)
        {
            Destroy (image.gameObject);
        }
        DisplayedItemsImages.Clear ();

        Interact ();
    }

    private void OpenChest ( )
    {
        _chestAC.SetTrigger ("Open");
        _playerAC.SetTrigger ("OpenChest");
        OpenChestPrompt.SetActive (false);
        InteractPrompt.SetActive (true);
        InteractPrompt.GetComponent<Button> ().Select ();
        ChestOpened = true;
    }

    private IEnumerator ShowChestContent(float time)
    {
        yield return new WaitForSeconds (time);

        ChestContentUIPanel.SetActive (!ChestContentUIPanel.activeSelf);            //Activate panel so we can see items 

        //Activate particle system inside the chest
    }
}
