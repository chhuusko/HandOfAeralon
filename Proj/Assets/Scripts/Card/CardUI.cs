
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI _title, _description, _mana;
    [SerializeField] Image _base, _frame, _image;
    [SerializeField] GameObject _glow;

    private Card _card;

    InfoPanelHandler _infoHandler;
    private void Awake()
    {
    
        _infoHandler = gameObject.GetComponent<InfoPanelHandler>();
    }
    private void OnEnable()
    {
        CardHandManager.onManaChange += UpdateGlow;
    }
    private void OnDisable()
    {
        CardHandManager.onManaChange -= UpdateGlow;
    }

    public void SetUpUIElements(Card card)
    {
        _card = card;
        _title.text = card.title;
        _description.text = GameTextFormatter.LabeledDescription(card.description);
        _mana.text = "" + card.GetCost();
        _image.sprite = card.icon;
        _frame.color = card.GetRarityColor((int)card.rarity);
        _base.color = card.GetRarityColor((int)card.rarity);
        if (_infoHandler != null)
        {
            _infoHandler.SetInfoPanel(card.info);
        }

        UpdateGlow();
    }
    public void SetUpUIElements(Card card, bool showInfoPanels)
    {
        _card = card;
        _title.text = card.title;
        _description.text = GameTextFormatter.LabeledDescription(card.description);
        _mana.text = "" + card.GetCost();
        _image.sprite = card.icon;
        _frame.color = card.GetRarityColor((int)card.rarity);
        _base.color = card.GetRarityColor((int)card.rarity);
        if (_infoHandler != null)
        {
            _infoHandler.SetInfoPanel(card.info);
        }
        // for zoomedcard
        _infoHandler.SetShowInfoPanel(showInfoPanels);
        UpdateGlow();
    }
    public void UpdateText()
    {
   
    }
    private void UpdateGlow()
    {
        if (_glow)
        {
            _glow.SetActive(CanAfford());
        }
        SetCardManaText();
    }
    private void UpdateGlow(int i)
    {
        UpdateGlow();
    }
    private bool CanAfford()
    {
        
        if (CardHandManager.GetInstance())
        {
            if (CardHandManager.GetInstance().GetMana() >= _card.GetCost())
            {
                return true;
            } 
        }
        return false;
    }
    private void SetCardManaText()
    {
        if (CardHandManager.GetInstance() == null)
        {
            _mana.text = "" + _card.GetCost();
            return;
        }
        if (CanAfford())
        {
            if (_card.GetIsTemp())
            {
                _mana.text = "<color=green>" + _card.GetCost() + "</color>";
            }
            else
            {
                _mana.text = "" + _card.GetCost();
            }
        }
        else
        {
            _mana.text = "<color=red>" + _card.GetCost() + "</color>";
        }
        
        
    }
    public Card GetCard()
    {
        return _card;
    }

}
