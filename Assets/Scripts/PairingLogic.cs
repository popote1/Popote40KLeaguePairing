using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PairingLogic : MonoBehaviour
{


    [SerializeField] private PlayerHandUI _currentPlayerHand;
    [SerializeField] private PlayerHandUI _opponentPlayerHand;

    [Space(5)]
    [SerializeField] private PropositionOneCartUI _propositionOneCartUI;
    [SerializeField] private PropositionTwoCartUI _propositionTwoCartUI;
    [SerializeField] private ChoseBetweenTwoCartUI _choseBetweenTwoCartUI;
    [SerializeField] private SubmitedPanelUI _submitedPanelUI;
    [SerializeField] private MatchRecapUI _matchRecapUI;
    [SerializeField] private Button _bpMatchRecap;
    [SerializeField] private List<SOCarte> _cartes;
    

    
    private int _pairingStat;
    private int _teamSize=6;
    private int _cartesPlay = 0;
    [SerializeField]private bool _isFinalState;

    private PlayerData _player1;
    private PlayerData _player2;
    private SOTeam _teamPlayer1;
    private SOTeam _teamPlayer2;

    private List<SOCarte> _currentPlayerSOCart;
    private List<SOCarte> _opponentPlayerSOCart;
    private List<int> _player1PlayerAssigned = new List<int>();
    private List<int> _player2PlayerAssigned = new List<int>();
    
    private bool _isOpponent;
    private int _localChose = -1;
    private int _selectedCart =-1;
    private int _selection1 = -1;
    private int _selection2 = -1;

    private int _opponentSelectedCart =-1;
    private int _opponentSelection1 = -1;
    private int _opponentSelection2 = -1;

    private bool _player1Ready;
    private bool _player2Ready;

    [SerializeField]private Tuple<int, int> _match1 = new Tuple<int, int>(-1, -1);
    [SerializeField]private Tuple<int, int> _match2 = new Tuple<int, int>(-1, -1);
    [SerializeField]private Tuple<int, int> _match3 = new Tuple<int, int>(-1, -1);
    [SerializeField]private Tuple<int, int> _match4 = new Tuple<int, int>(-1, -1);
    [SerializeField]private Tuple<int, int> _match5 = new Tuple<int, int>(-1, -1);
    [SerializeField]private Tuple<int, int> _match6 = new Tuple<int, int>(-1, -1);
    
    private bool IsAllReady() => _player1Ready && _player2Ready;

    private void Awake() {
        _currentPlayerHand.OnCarteSelected += CurrentPlayerHand_OnCartSelected;
        _propositionOneCartUI.OnReturn += PropositionOneCartUI_OnReturn;
        _propositionTwoCartUI.OnReturn1 += PropositionOneCartUI_OnReturn;
        _propositionTwoCartUI.OnReturn2 += PropositionOneCartUI_OnReturn2;
        _choseBetweenTwoCartUI.OnChosen += ChoseBetweenTwoCartUI_OnChose;

        _propositionOneCartUI.OnSubmit += PropositionOneCartUI_OnSubmit;
        _propositionTwoCartUI.OnSubmit += PropositionOneCartUI_OnSubmit;
        _choseBetweenTwoCartUI.OnSubmit += PropositionOneCartUI_OnSubmit;
        
        _bpMatchRecap.onClick.AddListener(ShowMatchRecap);
    }

    private void ShowMatchRecap() => _matchRecapUI.SetCurrentMatch(GetCurrentMatchState());
    

    private void ChoseBetweenTwoCartUI_OnChose(object sender, int e) {
        _localChose = e;
    }

    private void PropositionOneCartUI_OnSubmit(object sender, EventArgs e) {
        switch (_pairingStat) {
            case 1 : 
                PopoteNetPart.Instance.PairingSingleSubmitServerRpc(_selection1);
                _propositionOneCartUI.Hide();
                break;
            case 2 :
                PopoteNetPart.Instance.PairingDoubleSubmitServerRpc(_selection1, _selection2);
                _propositionTwoCartUI.Hide();
                break;
            case 3 :
                PopoteNetPart.Instance.ChoseSubmitServerRpc(_localChose);
                _choseBetweenTwoCartUI.Hide();
                break;
        }
    }

    private void PropositionOneCartUI_OnReturn2(object sender, EventArgs e) {
        _propositionOneCartUI.DisplayCart(null);
        _propositionTwoCartUI.DisplayCart2(null);
        if( _selection2!=-1) _currentPlayerHand.ShowCartOfIndex(_selection2);
        _selection2 = -1;
    }
    
    private void PropositionOneCartUI_OnReturn(object sender, EventArgs e) {
        _propositionOneCartUI.DisplayCart(null);
        _propositionTwoCartUI.DisplayCart1(null);
        if( _selection1!=-1) _currentPlayerHand.ShowCartOfIndex(_selection1); 
        _selection1 = -1;
    }

    private void CurrentPlayerHand_OnCartSelected(object sender, int id) {
        switch (_pairingStat) {
            case 1 : SelectionOneDefender(id);
                break;
            case 2 : SelectionTwoAttacker(id);
                break;
        }
    }

    public void StartParingStat() {
        if (_teamSize - _cartesPlay <= 4) _isFinalState = true;
        Debug.Log("Start Pairing Stat");
        
        switch (_pairingStat) {
            case 1 : _propositionOneCartUI.ClearShow();
                _submitedPanelUI.Hide();
                break;
            case 2 : _propositionTwoCartUI.ClearShow(); break;
            case 3 : 
                if (PopoteNetPart.Instance.IsServer) {
                    _choseBetweenTwoCartUI.DisplayCarts(_opponentPlayerSOCart[_opponentSelection1],_opponentPlayerSOCart[_opponentSelection2]);
                }
                else {
                    _choseBetweenTwoCartUI.DisplayCarts(_opponentPlayerSOCart[_selection1],_opponentPlayerSOCart[_selection2]);
                }

                break;
        }
    }
    public void SetupPairing(PlayerData player1, SerializeSOTeam team1,PlayerData player2, SerializeSOTeam team2) {
        _player1 = player1;
        _player2 = player2;
        _teamPlayer1 = ScriptableObject.CreateInstance<SOTeam>();
        _teamPlayer1.LoadDataFromSerializeData(team1);
        _teamPlayer2 = ScriptableObject.CreateInstance<SOTeam>();
        _teamPlayer2.LoadDataFromSerializeData(team2);
        
        if (player1.ClientId == PopoteNetPart.Instance.GetCurrentNetworkId()) {
            _currentPlayerSOCart = GetSoCartsFromTeam(_teamPlayer1);
            _opponentPlayerSOCart = GetSoCartsFromTeam(_teamPlayer2);
            _currentPlayerHand.SetTeam( _currentPlayerSOCart);
            _opponentPlayerHand.SetTeam( _opponentPlayerSOCart);
        }
        else {
            _currentPlayerSOCart = GetSoCartsFromTeam(_teamPlayer2);
            _opponentPlayerSOCart = GetSoCartsFromTeam(_teamPlayer1);
            _currentPlayerHand.SetTeam( _currentPlayerSOCart);
            _opponentPlayerHand.SetTeam( _opponentPlayerSOCart);
        }
        
        _currentPlayerHand.Show();
        _opponentPlayerHand.Show();
        _pairingStat = 1;
        StartParingStat();
    }

    public void ServerSingleSubmite(int carteId , ulong playerId) {
        if (playerId == PopoteNetPart.Instance.GetCurrentNetworkId()) {
            _selection1 = carteId;
            _player1Ready = true;
        }
        else {
            _opponentSelection1 = carteId;
            _player2Ready = true;
        }

        if (IsAllReady() && PopoteNetPart.Instance.IsServer) {
            PopoteNetPart.Instance.EndSimpleStatServerRpc(_selection1, _opponentSelection1);
            _player1Ready = _player2Ready = false;
        }
    }
    public void ServerDoubleSubmite(int carteId1,int carteId2 , ulong playerId) {
        if (playerId == PopoteNetPart.Instance.GetCurrentNetworkId()) {
            _selection1 = carteId1;
            _selection2 = carteId2;
            _player1Ready = true;
            Debug.Log("Player1 submited");
        }
        else {
            _opponentSelection1 = carteId1;
            _opponentSelection2 = carteId2;
            _player2Ready = true;
            Debug.Log("Player2 submited");
        }

        if (IsAllReady() && PopoteNetPart.Instance.IsServer) {
            PopoteNetPart.Instance.EndDoubleStatServerRpc(_selection1, _selection2, _opponentSelection1, _opponentSelection2);
            _player1Ready = _player2Ready = false;
        }
    }

    public void ServerChoseSubmite(int carteId, ulong playerId) {
        if (playerId == PopoteNetPart.Instance.GetCurrentNetworkId()) {
            _selectedCart = carteId;
            _player1Ready = true;
        }
        
        else {
            _opponentSelectedCart = carteId;
            _player2Ready = true;
        }

        if (IsAllReady() && PopoteNetPart.Instance.IsServer) {
            PopoteNetPart.Instance.EndChoseStatServerRpc(_selectedCart, _opponentSelectedCart);
            _player1Ready = _player2Ready = false;
        }
    }
    private List<SOCarte> GetSoCartsFromTeam(SOTeam team) {
        List<SOCarte> cartes = new List<SOCarte>();
        foreach (var carteName in team.Factions) {
            foreach (var carte in _cartes) {
                if( carte.Name == carteName) cartes.Add(carte);
            }
        }
        return cartes;
    }

    private void SelectionOneDefender(int id) {
        if( _selection1!=-1)_currentPlayerHand.ShowCartOfIndex(_selection1);
        _selection1 = id;
        _currentPlayerHand.HideCartOfIndex(id); 
        _propositionOneCartUI.DisplayCart(_currentPlayerSOCart[id]);
    }
    
    private void SelectionTwoAttacker(int id) {
        if (_selection1 != -1) {
            if (_selection2 != -1) {
                _currentPlayerHand.ShowCartOfIndex(_selection2);
            }
            _selection2 = id;
            _propositionTwoCartUI.DisplayCart2(_currentPlayerSOCart[id]);
            _currentPlayerHand.HideCartOfIndex(id);
            return;
        }
        _selection1 = id;
        _propositionTwoCartUI.DisplayCart1(_currentPlayerSOCart[id]);
        _currentPlayerHand.HideCartOfIndex(id);
    }
     public void EndSimpleStat(int cartPlayer1 ,int cartPlayer2 ) {
        switch (_pairingStat) {
            case 1:
                if (_isFinalState) {
                    _match3 = new Tuple<int, int>(cartPlayer1, -1);
                    _match4 = new Tuple<int, int>(-1, cartPlayer2);
                }
                else {
                    _match1 = new Tuple<int, int>(cartPlayer1, -1);
                    _match2 = new Tuple<int, int>(-1, cartPlayer2);
                }

                if (PopoteNetPart.Instance.IsServer) {
                    _opponentPlayerHand.HideCartOfIndex(cartPlayer2);
                    _submitedPanelUI.DisplayCart1(_opponentPlayerSOCart[cartPlayer2]);
                }
                else {
                    _opponentPlayerHand.HideCartOfIndex(cartPlayer1);
                    _submitedPanelUI.DisplayCart1(_opponentPlayerSOCart[cartPlayer1]);
                }
                _selection1 = -1;
                _opponentSelection1 = -1;
                
                _pairingStat = 2;
                StartParingStat();
                _submitedPanelUI.Show();
                
                break;
        }
    }

    public void EndDoubleStat(int cart1Player1, int cart2Player1, int cart1Player2, int cart2Player2)
    {
        switch (_pairingStat) {
            case 2:
                Debug.Log("Try Ending Double State with index of"+ cart1Player1+ cart1Player2+ cart2Player1+ cart2Player2);
                if (PopoteNetPart.Instance.IsServer) {
                    _opponentPlayerHand.HideCartOfIndex(cart1Player2);
                    _opponentPlayerHand.HideCartOfIndex(cart2Player2);
                }
                else {
                    _opponentPlayerHand.HideCartOfIndex(cart1Player1);
                    _opponentPlayerHand.HideCartOfIndex(cart2Player1);
                }
                _selection1 = cart1Player1;
                _selection2 = cart2Player1;
                _opponentSelection1 = cart1Player2;
                _opponentSelection2 = cart2Player2;

                _pairingStat = 3;
                StartParingStat();
                break;
        }
    }

    public void EndChoseState(int chosenCartPlayer1, int chosenCartPlayer2) {
        switch (_pairingStat) {
            case 3 :
                if (_isFinalState) {
                    EndPairing(chosenCartPlayer1, chosenCartPlayer2);
                    break;
                }
                EndNormalChoseStat(chosenCartPlayer1, chosenCartPlayer2);
                break;
        }
    }

    private void EndNormalChoseStat(int chosenCartPlayer1, int chosenCartPlayer2) {
        if (PopoteNetPart.Instance.IsServer) {
            if (chosenCartPlayer1 == 1) _currentPlayerHand.ShowCartOfIndex(_selection2);
            else if (chosenCartPlayer1 == 2) _currentPlayerHand.ShowCartOfIndex(_selection1);
                    
            if (chosenCartPlayer2 == 1) _opponentPlayerHand.ShowCartOfIndex(_opponentSelection2);
            else if (chosenCartPlayer2 == 2) _opponentPlayerHand.ShowCartOfIndex(_opponentSelection1);
        }
        else
        {
            if (chosenCartPlayer1 == 1) _opponentPlayerHand.ShowCartOfIndex(_selection2);
            else if (chosenCartPlayer1 == 2) _opponentPlayerHand.ShowCartOfIndex(_selection1);

            if (chosenCartPlayer2 == 1) _currentPlayerHand.ShowCartOfIndex(_opponentSelection2);
            else if (chosenCartPlayer2 == 2) _currentPlayerHand.ShowCartOfIndex(_opponentSelection1);
        }

        if (chosenCartPlayer1 == 1) {
                    _match2=new Tuple<int, int>(_selection1, _match2.Item2);
                    _player1PlayerAssigned.Add(_selection1);
                    _player2PlayerAssigned.Add(_match2.Item2);
        }
        else if (chosenCartPlayer1 == 2) {
            _match2=new Tuple<int, int>(_selection2, _match2.Item2);
            _player1PlayerAssigned.Add(_selection2);
            _player2PlayerAssigned.Add(_match2.Item2);
        }

        if (chosenCartPlayer2 == 1) {
            _match1=new Tuple<int, int>(_match1.Item1,_opponentSelection1);
            _player1PlayerAssigned.Add(_match1.Item1);
            _player2PlayerAssigned.Add(_opponentSelection1);
        }
        else if (chosenCartPlayer2 == 2) {
            _match1=new Tuple<int, int>(_match1.Item1,_opponentSelection2 );
            _player1PlayerAssigned.Add(_match1.Item1);
            _player2PlayerAssigned.Add(_opponentSelection2);
        }

        _selectedCart = -1;
        _selection1 = -1;
        _selection2 = -1;
        _opponentSelectedCart = -1;
        _opponentSelection1 = -1;
        _opponentSelection1 = -1;

        _pairingStat = 1;
        _cartesPlay += 2;
        StartParingStat();
    }

    private void EndPairing(int chosenCartPlayer1, int chosenCartPlayer2)
    {
        Tuple<int, int> refusedMatch = new Tuple<int, int>(-1,-1) ;
        
        if (chosenCartPlayer1 == 1) {
            _match4=new Tuple<int, int>(_selection1, _match4.Item2);
            _player1PlayerAssigned.Add(_selection1);
            _player2PlayerAssigned.Add(_match4.Item2);
            refusedMatch = new Tuple<int, int>(_selection2, -1);

        }
        else if (chosenCartPlayer1 == 2) {
            _match4=new Tuple<int, int>(_selection2, _match4.Item2);
            _player1PlayerAssigned.Add(_selection2);
            _player2PlayerAssigned.Add(_match4.Item2);
            refusedMatch = new Tuple<int, int>(_selection1, -1);
        }

        if (chosenCartPlayer2 == 1) {
            _match3=new Tuple<int, int>(_match3.Item1,_opponentSelection1);
            _player1PlayerAssigned.Add(_match3.Item1);
            _player2PlayerAssigned.Add(_opponentSelection1);
            refusedMatch = new Tuple<int, int>(refusedMatch.Item1, _opponentSelection2);
        }
        else if (chosenCartPlayer2 == 2) {
            _match3=new Tuple<int, int>(_match3.Item1,_opponentSelection2 );
            _player1PlayerAssigned.Add(_match3.Item1);
            _player2PlayerAssigned.Add(_opponentSelection2);
            refusedMatch = new Tuple<int, int>(refusedMatch.Item1, _opponentSelection1);
        }
        Tuple<int, int> forgotenMatch = new Tuple<int, int>(GetNotAssignPlayer(_player1PlayerAssigned), GetNotAssignPlayer(_player2PlayerAssigned));

        _match5 = refusedMatch;
        _match6 = forgotenMatch;
        
        _selectedCart = -1;
        _selection1 = -1;
        _selection2 = -1;
        _opponentSelectedCart = -1;
        _opponentSelection1 = -1;
        _opponentSelection1 = -1;
        _matchRecapUI.SetCurrentMatch(GetCurrentMatchState());

    }

    private List<Tuple<SOCarte, SOCarte>> GetCurrentMatchState() {
        List<SOCarte> t1 ;
        List<SOCarte> t2 ;
        if (PopoteNetPart.Instance.IsServer) {
            t1 = _currentPlayerSOCart;
            t2 = _opponentPlayerSOCart;
        }
        else {
            t1 = _opponentPlayerSOCart;
            t2 = _currentPlayerSOCart;
        }

        List<Tuple<SOCarte, SOCarte>> returnList = new List<Tuple<SOCarte, SOCarte>>();
        returnList.Add(GetMatchSoCarteTuple(_match1, t1, t2));
        returnList.Add(GetMatchSoCarteTuple(_match2, t1, t2));
        returnList.Add(GetMatchSoCarteTuple(_match3, t1, t2));
        returnList.Add(GetMatchSoCarteTuple(_match4, t1, t2));
        returnList.Add(GetMatchSoCarteTuple(_match5, t1, t2));
        returnList.Add(GetMatchSoCarteTuple(_match6, t1, t2));
        return returnList;
        //returnList.Add(new Tuple<SOCarte, SOCarte>(t1[_match7.Item1], t2[_match1.Item2]));
    }

    private Tuple<SOCarte, SOCarte> GetMatchSoCarteTuple(Tuple<int, int> match, List<SOCarte> t1, List<SOCarte> t2) {
        if (match.Item1==-1 || match.Item2==-1) return new Tuple<SOCarte, SOCarte>(null, null);
        return new Tuple<SOCarte, SOCarte>(t1[match.Item1], t2[match.Item2]);
    }

    private int GetNotAssignPlayer(List<int> team) {
        for (int i = 0; i < _teamSize; i++) {
            if (team.Contains(i)) continue;
            return i;
        }
        return -1;
    }
}
