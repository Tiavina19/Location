using System;
using System.Collections.Generic;
using System.Globalization;

namespace RentalSystem
{
    public class Reservation
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public Reservation(DateTime startDate, int days)
        {
            if (days < 1)
                throw new ArgumentException("La réservation doit durer au moins 1 jour.");

            StartDate = startDate.Date;
            EndDate = StartDate.AddDays(days);
        }

        public bool ConflictsWith(DateTime start, DateTime end)
        {
            return start < EndDate && end > StartDate;
        }
    }

    public class RentableItem
    {
        public string Name { get; }
        private readonly List<Reservation> _reservations = new List<Reservation>();

        public RentableItem(string name)
        {
            Name = name;
        }

        public (bool Success, string Message) Reserve(DateTime startDate, int days)
        {
            DateTime desiredStart = startDate.Date;
            DateTime desiredEnd = desiredStart.AddDays(days);

            // Détection des conflits
            List<Reservation> conflicts = new List<Reservation>();
            foreach (var r in _reservations)
            {
                if (r.ConflictsWith(desiredStart, desiredEnd))
                    conflicts.Add(r);
            }

            if (conflicts.Count > 0)
            {
                DateTime nextAvailable = conflicts[0].EndDate;
                foreach (var r in conflicts)
                {
                    if (r.EndDate > nextAvailable)
                        nextAvailable = r.EndDate;
                }
                return (false, string.Format("Non disponible. Prochain créneau disponible à partir du {0:dd/MM/yyyy}.", nextAvailable));
            }

            var newRes = new Reservation(desiredStart, days);
            _reservations.Add(newRes);
            return (true, string.Format("Réservation confirmée du {0:dd/MM/yyyy} au {1:dd/MM/yyyy}.", desiredStart, desiredEnd));
        }

        public IEnumerable<Reservation> GetReservations()
        {
            foreach (var r in _reservations)
                yield return r;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            var items = new List<RentableItem>
            {
                new RentableItem("Voiture Kia Sportage 4"),
                new RentableItem("Appartement T2"),
                new RentableItem("Vélo de ville"),
                new RentableItem("Scooter SYM JET S"),
                new RentableItem("Set de vaisselle (8 couverts)"),
                new RentableItem("Espace 800 mètre carré")

            };

            Console.WriteLine("=== Système de Réservation ===");
            bool continuerProgramme = true;

            while (continuerProgramme)
            {

                Console.WriteLine("Choisissez un objet à réserver :");
                for (int i = 0; i < items.Count; i++)
                    Console.WriteLine($"{i + 1}. {items[i].Name}");

                Console.Write("Numéro de l'objet : ");
                string choix = Console.ReadLine();
                int index;
                if (!int.TryParse(choix, out index) || index < 1 || index > items.Count)
                {
                    Console.WriteLine("Choix invalide.");
                    continue;
                }

                RentableItem selected = items[index - 1];
                Console.WriteLine($"Vous avez choisi : {selected.Name}");


                Console.Write("Date de début (jj/MM/yyyy) : ");
                string dateInput = Console.ReadLine();
                Console.Write("Durée en jours (>=1) : ");
                string daysInput = Console.ReadLine();

                DateTime startDate;
                int days;
                if (DateTime.TryParseExact(dateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate)
                    && int.TryParse(daysInput, out days))
                {
                    try
                    {
                        var res = selected.Reserve(startDate, days);
                        Console.WriteLine(res.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Entrée invalide. Veuillez respecter les formats.");
                }


                Console.Write("Souhaitez-vous faire un autre réservation ? (o/n) : ");
                string rep = Console.ReadLine();
                continuerProgramme = (rep != null && rep.ToLower() == "o");
            }

            Console.WriteLine("Merci pour votre confiance, à bientôt !");
        }
    }
}
