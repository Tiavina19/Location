package main

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
	"strings"
	"time"
)


type Reservation struct {
	StartDate time.Time
	EndDate   time.Time
}


func (r Reservation) ConflictsWith(start, end time.Time) bool {
	return start.Before(r.EndDate) && end.After(r.StartDate)
}


type RentableItem struct {
	Name         string
	Reservations []Reservation
}


func (item *RentableItem) Reserve(startDate time.Time, days int) (bool, string) {
	if days < 1 {
		return false, "La réservation doit durer au moins 1 jour."
	}

	desiredStart := startDate.Truncate(24 * time.Hour)
	desiredEnd := desiredStart.AddDate(0, 0, days)

	
	var conflicts []Reservation
	for _, r := range item.Reservations {
		if r.ConflictsWith(desiredStart, desiredEnd) {
			conflicts = append(conflicts, r)
		}
	}

	if len(conflicts) > 0 {
		nextAvailable := conflicts[0].EndDate
		for _, r := range conflicts {
			if r.EndDate.After(nextAvailable) {
				nextAvailable = r.EndDate
			}
		}
		return false, fmt.Sprintf("Non disponible. Prochain créneau disponible à partir du %s.", nextAvailable.Format("02/01/2006"))
	}

	
	r := Reservation{StartDate: desiredStart, EndDate: desiredEnd}
	item.Reservations = append(item.Reservations, r)
	return true, fmt.Sprintf("Réservation confirmée du %s au %s.", desiredStart.Format("02/01/2006"), desiredEnd.Format("02/01/2006"))
}

func main() {
	scanner := bufio.NewScanner(os.Stdin)
	
	items := []*RentableItem{
		{Name: "Voiture Kia Sportage 4"},
		{Name: "Appartement T2"},
		{Name: "Vélo de ville"},
		{Name: "Scooter SYM JET S"},
		{Name: "Set de vaisselle (8 couverts)"},
		{Name: "Espace 800 mètre carré"},
	}

	fmt.Println("=== Système de Réservation ===")
	for {
		
		fmt.Println("Choisissez un objet à réserver :")
		for i, it := range items {
			fmt.Printf("%d. %s\n", i+1, it.Name)
		}

		fmt.Print("Numéro de l'objet : ")
		scanner.Scan()
		choice := scanner.Text()
		idx, err := strconv.Atoi(strings.TrimSpace(choice))
		if err != nil || idx < 1 || idx > len(items) {
			fmt.Println("Choix invalide.")
			continue
		}
		selected := items[idx-1]
		fmt.Printf("Vous avez choisi : %s\n", selected.Name)

		
		fmt.Print("Date de début (jj/MM/yyyy) : ")
		scanner.Scan()
		dateInput := strings.TrimSpace(scanner.Text())
		
		fmt.Print("Durée en jours (>=1) : ")
		scanner.Scan()
		daysInput := strings.TrimSpace(scanner.Text())

		startDate, errDate := time.Parse("02/01/2006", dateInput)
		days, errDays := strconv.Atoi(daysInput)
		if errDate != nil || errDays != nil {
			fmt.Println("Entrée invalide. Veuillez respecter les formats.")
			continue
		}

		
		_, msg := selected.Reserve(startDate, days)
		fmt.Println(msg)

		
		fmt.Print("Souhaitez-vous faire une autre réservation ? (o/n) : ")
		scanner.Scan()
		rep := strings.ToLower(strings.TrimSpace(scanner.Text()))
		if rep != "o" {
			break
		}
	}

	fmt.Println("Merci pour votre confiance, à bientôt !")
}
