<?php

function parseDate($input) {
    $d = DateTime::createFromFormat('d/m/Y', $input);
    return $d && $d->format('d/m/Y') === $input ? $d : false;
}

function addDays($date, $days) {
    $copy = clone $date;
    return $copy->modify("+$days days");
}

$items = [
    ["name" => "Voiture Kia Sportage 4", "reservations" => []],
    ["name" => "Appartement T2", "reservations" => []],
    ["name" => "Vélo de ville", "reservations" => []],
    ["name" => "Scooter SYM JET S", "reservations" => []],
    ["name" => "Set de vaisselle", "reservations" => []],
    ["name" => "Espace 800m²", "reservations" => []],
];

while (true) {
    echo "\n=== Réservation ===\n";
    foreach ($items as $i => $item) {
        echo ($i + 1) . ". " . $item["name"] . "\n";
    }

    echo "Ecrire ici le numéro qui contient votre choix : ";
    $choice = trim(fgets(STDIN));
    if (!is_numeric($choice) || $choice < 1 || $choice > count($items)) break;

    $index = $choice - 1;
    echo "Date jj/mm/yyyy: ";
    $dateInput = trim(fgets(STDIN));
    echo "Location pendant combien de jours ? : ";
    $daysInput = trim(fgets(STDIN));
    $startDate = parseDate($dateInput);
    $days = intval($daysInput);

    if (!$startDate || $days < 1) {
        echo "Entrée invalide.\n";
        continue;
    }

    $endDate = addDays($startDate, $days);
    $conflict = false;

    foreach ($items[$index]["reservations"] as $res) {
        if ($startDate < $res["end"] && $endDate > $res["start"]) {
            echo "Non disponible en ce moment. Prochain: " . $res["end"]->format("d/m/Y") . "\n";
            $conflict = true;
            break;
        }
    }

    if (!$conflict) {
        $items[$index]["reservations"][] = ["start" => $startDate, "end" => $endDate];
        echo "Réservé du " . $startDate->format("d/m/Y") . " au " . $endDate->format("d/m/Y") . "\n";
    }

    echo "SOuhaitez vous faire d'autre reservation ? (o/n): ";
    $rep = trim(fgets(STDIN));
    if (strtolower($rep) !== "o") break;
}

echo "Merci pour votre confiance, à bientôt !\n";
