using System;
using Stateless;

namespace App.ServiceInterface
{
    public class Bug
    {
        public enum State
        {
            Open,
            Assigned,
            Deferred,
            Resolved,
            Closed
        }

        public enum Trigger
        {
            Assign,
            Defer,
            Resolve,
            Close
        }

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _assignTrigger;
        private readonly StateMachine<State, Trigger> _machine;

        private readonly string _title;
        private string _assignee;
        private State _state = State.Open;

        #region Constructor

        public Bug(string title)
        {
            _title = title; 

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _assignTrigger = _machine.SetTriggerParameters<string>(Trigger.Assign);

            _machine.Configure(State.Open)
                .Permit(Trigger.Assign, State.Assigned);

            _machine.Configure(State.Assigned)
                .SubstateOf(State.Open)
                .OnEntryFrom(_assignTrigger, assignee => OnAssigned(assignee))
                .PermitReentry(Trigger.Assign)
                .Permit(Trigger.Close, State.Closed)
                .Permit(Trigger.Defer, State.Deferred)
                .OnExit(() => OnDeassigned());

            _machine.Configure(State.Deferred)
                .OnEntry(() => _assignee = null)
                .Permit(Trigger.Assign, State.Assigned);
        }

        #endregion

        #region Public methods

        public bool CanAssign
        {
            get { return _machine.CanFire(Trigger.Assign); }
        }

        public State CurrentState()
        {
            return _machine.State;
        }

        public string Assignee()
        {
            return _assignee;
        }

        public void Close()
        {
            _machine.Fire(Trigger.Close);
            _assignee = null;
        }

        public void Assign(string assignee)
        {
            _machine.Fire(_assignTrigger, assignee);
        }

        public void Defer()
        {
            _machine.Fire(Trigger.Defer);
        }

        #endregion

        #region Private methods

        private void OnAssigned(string assignee)
        {
            if (_assignee != null && assignee != _assignee)
            {
                SendEmailToAssignee("Don't forget to help the new guy."); 
            }              
            _assignee = assignee;
            SendEmailToAssignee("You own it.");
        }


        private void OnDeassigned()
        {
            SendEmailToAssignee("You're off the hook.");
        }

        private void SendEmailToAssignee(string message)
        {
            Console.WriteLine("{0}, RE {1}: {2}", _assignee, _title, message);
        }

        #endregion
    }
}