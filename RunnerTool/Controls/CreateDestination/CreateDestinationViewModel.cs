using PropertyChanged;
using RunnerTool.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;

namespace RunnerTool
{
	/// <summary>
	/// 
	/// </summary>
	public class CreateDestinationViewModel : BaseVisibleViewModel
	{
		#region privamte members

		Destination curDestination;
		string searchText;
		List<BaseInputViewModel> inputVMs;

		#endregion

		#region public properties

		public override bool IsVisible { get; set; } = false;

		//list section
		public List<DestinationListItemViewModel> DestinationVMs { get; private set; }
		public ObservableCollection<DestinationListItemViewModel> FilteredDestinationVMs { get; private set; }

		[DoNotCheckEquality]
		public Destination CurDestination
		{
			get => curDestination;
			set
			{
				curDestination = value;
				UpdateInput();
			}
		}

		public string TitleText => (CurDestination == null ? "createDestination" : "editDestination");

		public string OkButtonText => (CurDestination == null ? "create" : "finish");

		public bool IsInputSectionVisible { get; private set; }

		[DoNotCheckEquality]
		public string SearchText
		{
			get => searchText;
			set
			{
				searchText = value;
				RecalcFilteredDestinations();
			}
		}

		//create/edit section
		public StringInputViewModel NameInputVM { get; private set; }
		public StringInputViewModel ShortNameInputVM { get; private set; }
		public ComboBoxInputViewModel DestinationTypeInputVM { get; private set; }
		public CheckBoxInputViewModel IsGoodieGiverInputVM { get; private set; }
		public DayOfWeekSelectionInputViewModel DayOfWeekSelectionInputVM { get; private set; }

		#endregion

		#region commands

		public ICommand ResetCommand { get; private set; }
		public ICommand OkButtonCommand { get; private set; }
		public ICommand BeginCreationCommand { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public CreateDestinationViewModel ()
		{
			DestinationVMs = new List<DestinationListItemViewModel>();
			FilteredDestinationVMs = new ObservableCollection<DestinationListItemViewModel>();
			UpdateDestinationVMs();

			SearchText = "";

			//input fields
			NameInputVM = new StringInputViewModel("name");
			ShortNameInputVM = new StringInputViewModel("shortName");
			DestinationTypeInputVM = new ComboBoxInputViewModel("type", System.Enum.GetNames(typeof(DestinationType)));
			IsGoodieGiverInputVM = new CheckBoxInputViewModel("goodieGiver");
			DayOfWeekSelectionInputVM = new DayOfWeekSelectionInputViewModel("closed");

			inputVMs = new List<BaseInputViewModel>
			{
				NameInputVM,
				ShortNameInputVM,
				DestinationTypeInputVM,
				IsGoodieGiverInputVM,
				DayOfWeekSelectionInputVM,
			};

			//commands
			ResetCommand = new RelayCommand(ResetCommandMethod);
			OkButtonCommand = new RelayCommand(OkButtonCommandMethod);
			BeginCreationCommand = new RelayCommand(BeginCreationCommandMethod);

			//register events
			DB.Instance.OnDestinationsChanged += UpdateDestinationVMs;
		}

		~CreateDestinationViewModel ()
		{
			//unregister events
			DB.Instance.OnDestinationsChanged -= UpdateDestinationVMs;
		}

		#endregion

		#region command methods

		void ResetCommandMethod ()
		{
			ResetInput();
		}

		void OkButtonCommandMethod ()
		{
			//no current destination; create new destination
			if (CurDestination == null)
			{
				//create new destination
				Destination newDestination = new Destination(
						NameInputVM.Text,
						ShortNameInputVM.Text,
						EnumHelpers.ValueByIndex<DestinationType>(DestinationTypeInputVM.SelectedIndex),
						new List<DateConstraint> {
							new DayOfWeekDateConstraint(DayOfWeekSelectionInputVM.DayOfWeekSelections.
								Where(d => d.IsSelected).Select(d => d.DayOfWeek).ToList())
						},
						IsGoodieGiverInputVM.IsChecked);

				//add to database; callback will update and sort list
				DB.Instance.AddDestination(newDestination);
			}
			//edit existing destination
			else
			{
				CurDestination.Name = NameInputVM.Text;
				CurDestination.ShortName = ShortNameInputVM.Text;
				CurDestination.Type = EnumHelpers.ValueByIndex<DestinationType>(DestinationTypeInputVM.SelectedIndex);
				CurDestination.IsGoodieGiver = IsGoodieGiverInputVM.IsChecked;
				CurDestination.ClosedDaysConstraint.SetDays(DayOfWeekSelectionInputVM.DayOfWeekSelections.
					Where(d => d.IsSelected).Select(d => d.DayOfWeek).ToArray());


				//force listeners to reload destination list
				DB.Instance.CallOnDestinationsChanged();
			}

			//changes might affect filtered list; therefore recalc
			RecalcFilteredDestinations();

			ResetInput();
		}

		void BeginCreationCommandMethod ()
		{
			BeginCreation();
		}

		#endregion

		#region public methods

		public void ResetInput ()
		{
			CurDestination = null;

			//reset input fields
			foreach (var inputVM in inputVMs)
			{
				inputVM.Reset();
			}

			IsInputSectionVisible = false;
		}

		public void UpdateInput ()
		{
			if (CurDestination == null)
				return;

			NameInputVM.Text = CurDestination.Name;
			ShortNameInputVM.Text = CurDestination.ShortName;
			DestinationTypeInputVM.SelectedIndex = EnumHelpers.IndexOf(CurDestination.Type);
			IsGoodieGiverInputVM.IsChecked = CurDestination.IsGoodieGiver;
			DayOfWeekSelectionInputVM.Update(curDestination.ClosedDaysConstraint);

			IsInputSectionVisible = true;
		}

		/// <summary>
		/// Resets and shows the input fields to create a new destination
		/// </summary>
		public void BeginCreation ()
		{
			ResetInput();

			IsInputSectionVisible = true;
		}

		#endregion

		#region private methods

		void UpdateDestinationVMs ()
		{
			//retrieve all destinations not already contained in destination view models
			DB.Instance.Destinations.Except(DestinationVMs.Select(d => d.Destination)).ToList().ForEach(d =>
			{
				//add new entries to list
				DestinationVMs.Add(new DestinationListItemViewModel(d)
				{
					//set callbacks for new entry
					LeftClickCallback = (destination) => CurDestination = destination
				});
			});
			DestinationVMs.Sort(Comparators.DestinationListItemViewModelCompare);

			//recalculate filtered items as well
			RecalcFilteredDestinations();
		}

		void RecalcFilteredDestinations ()
		{
			//get rid of previous selections
			FilteredDestinationVMs.Clear();

			//loop over all destinations...
			foreach (var item in DestinationVMs)
			{
				//add those, matching search text to list
				if (string.IsNullOrEmpty(SearchText) ||
						item.Destination.Name.ToLower().Contains(SearchText.ToLower()) ||
						item.Destination.ShortName.ToLower().Contains(SearchText.ToLower()))
				{
					FilteredDestinationVMs.Add(item);
				}
			}

			//add create new destination button to list
			FilteredDestinationVMs.Add(new NewDestinationListItemViewModel());
		}

		#endregion
	}
}