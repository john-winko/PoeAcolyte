﻿using System.Drawing;
using System.Windows.Forms;
using PoeAcolyte.API.Parsers;
using PoeAcolyte.UI;
using PoeAcolyte.UI.Interactions;

namespace PoeAcolyte.API.Interactions
{
    public class PoeSingleTrade : PoeWhisper
    {
        private SingleTradeUI _ui;
        public override UserControl InteractionUI => _ui;
        private bool _playerInArea;

        // TODO change to better functionality
        public override bool PlayerInArea
        {
            get => _playerInArea;
            set
            {
                _playerInArea = value;
                _ui.PlayerLabel.BackColor = _playerInArea ? Color.Aqua : SystemColors.Control;
            }
        }

        private bool _traderInArea;

        public override bool TraderInArea
        {
            get => _traderInArea;
            set
            {
                _traderInArea = value;
                _ui.LocationLabel.BackColor = _traderInArea ? Color.Aqua : SystemColors.Control;
            }
        }

        public PoeSingleTrade(IPoeLogEntry entry) : base(entry)
        {
            _ui = new SingleTradeUI(this);
            
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (Entry.Incoming)
            {
                _ui.IncomingLabel.Text = "Incoming";
                _ui.IncomingLabel.BackColor = Color.LightBlue;
                _ui.BackColor = Color.Pink;
            }
            else
            {
                _ui.IncomingLabel.Text = "Outgoing";
                _ui.IncomingLabel.BackColor = Color.LightYellow;
                _ui.BackColor = Color.LightGreen;
            }

            _ui.PlayerLabel.Text = Entry.Player;

            _ui.PriceLabel.Text = Entry.PoeLogEntryType == IPoeLogEntry.PoeLogEntryTypeEnum.UnpricedTrade ? 
                "Unpriced" : $"{Entry.PriceAmount} {Entry.PriceUnits}";

            _ui.LocationLabel.Text = $"({Entry.Top}, {Entry.Left}) {Entry.StashTab}";

            _ui.ToolTipHistory.SetToolTip(_ui.QuickButton, MessageHistory);
        }

        public override bool ShouldAdd(IPoeInteraction interaction)
        {
            if (interaction.Entry.PoeLogEntryType != IPoeLogEntry.PoeLogEntryTypeEnum.Whisper && 
                interaction.Entry.PoeLogEntryType != IPoeLogEntry.PoeLogEntryTypeEnum.PricedTrade &&
                interaction.Entry.PoeLogEntryType != IPoeLogEntry.PoeLogEntryTypeEnum.UnpricedTrade) return false;

            // whisper from same player
            if (interaction.Entry.Player == Entry.Player &&
                interaction.Entry.PoeLogEntryType == IPoeLogEntry.PoeLogEntryTypeEnum.Whisper)
            {
                History.Add(interaction);
                UpdateUI();
                return true;
            }

            // TODO add logic for duplicate trade requests
            return false;
        }

        
    }
}