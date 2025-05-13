#include <iostream>
#include <vector>
#include <string>
#include <ctime>
#include <sstream>
#include <iomanip>

bool parseDate(const std::string &s, std::tm &t)
{
    std::istringstream(s) >> std::get_time(&t, "%d/%m/%Y");
    return !std::istringstream(s).fail();
}

std::tm addDays(const std::tm &d, int days)
{
    auto tt = std::mktime(const_cast<std::tm *>(&d));
    tt += days * 86400;
    return *std::localtime(&tt);
}

struct Item
{
    std::string name;
    std::vector<std::pair<std::tm, std::tm>> res;
};

int main()
{
    std::vector<Item> items = {{"Voiture Kia Sportage 4"}, {"Appartement T2"}, {"Vélo de ville"}, {"Scooter SYM JET S"}, {"Set de vaisselle"}, {"Espace 800m²"}};
    char c;
    while (true)
    {
        std::cout << "=== Réservation ===\n";
        for (int i = 0; i < items.size(); i++)
            std::cout << i + 1 << ". " << items[i].name << "\n";
        std::cout << "Écrivez ici le numéro du type de location que vous souhaitez : ";
        int i;
        std::cin >> i;
        if (!std::cin || i < 1 || i > items.size())
            break;
        auto &it = items[i - 1];
        std::tm d = {};
        std::cout << "Date jj/mm/yyyy: ";
        std::cin >> std::get_time(&d, "%d/%m/%Y");
        int days;
        std::cout << "Pour combien de jours ? : ";
        std::cin >> days;
        auto start = d, end = addDays(d, days);
        bool ok = true;
        time_t s0 = std::mktime(&start), e0 = std::mktime(&end);
        for (auto &p : it.res)
        {
            time_t s1 = std::mktime(&p.first), e1 = std::mktime(&p.second);
            if (s0 < e1 && e0 > s1)
            {
                std::cout << "Non disponible, Prochain: " << std::put_time(&p.second, "%d/%m/%Y") << "\n";
                ok = false;
                break;
            }
        }
        if (ok)
        {
            it.res.emplace_back(start, end);
            std::cout << "Réservation du " << std::put_time(&start, "%d/%m/%Y") << " au "
                      << std::put_time(&end, "%d/%m/%Y") << "\n";
        }
        std::cout << "Souhaitez-vous faire d'autre réservation ? (o/n): ";
        std::cin >> c;
        if (c != 'o')
            break;
    }
}
