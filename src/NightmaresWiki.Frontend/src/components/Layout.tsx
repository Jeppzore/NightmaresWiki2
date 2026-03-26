import { NavLink, Outlet } from "react-router-dom";
import { SearchBox } from "./SearchBox";

const navigation = [
  { to: "/", label: "Home", end: true },
  { to: "/enemies", label: "Enemies" },
  { to: "/items", label: "Items" },
  { to: "/npcs", label: "NPC" },
];

const headerImages = [
  { src: "/images/header/armor.png", alt: "Armor" },
  { src: "/images/header/weapon.png", alt: "Weapon" },
  { src: "/images/header/bossTest.png", alt: "Boss" },
  { src: "/images/header/redMushroom.png", alt: "Mushroom" },
  { src: "/images/header/enemyTest.png", alt: "Enemy" },
];

export function Layout() {
  return (
    <div className="site-shell">
      <header className="site-header">
        <div className="site-header__inner">
          <div className="site-header__top">
            <div className="site-brand">
              <p className="site-brand__eyebrow">NightmaresWiki2</p>
              <h1>Nightmares Wiki</h1>
              <p className="site-brand__copy">
                A darker field guide for the creatures, loot, and lore imported from the legacy Nightmares archive.
              </p>
            </div>
            <SearchBox />
          </div>

          <div className="site-header__art" aria-label="Header artwork">
            {headerImages.map((image) => (
              <img key={image.src} src={image.src} alt={image.alt} />
            ))}
          </div>

          <nav className="site-nav" aria-label="Primary">
            {navigation.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.end}
                className={({ isActive }) => (isActive ? "site-nav__link site-nav__link--active" : "site-nav__link")}
              >
                {item.label}
              </NavLink>
            ))}
          </nav>

          <p className="site-header__note">
            Inspired by TibiaWiki/Fandom page structure and the original Nightmares header icon strip.
          </p>
        </div>
      </header>

      <div className="page-frame">
        <main className="page-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
