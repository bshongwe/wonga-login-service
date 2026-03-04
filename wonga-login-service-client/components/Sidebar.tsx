"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Home, LogIn, UserPlus, User } from "lucide-react";

export default function Sidebar() {
  const pathname = usePathname();

  const links = [
    { href: "/", label: "Home", icon: Home },
    { href: "/login", label: "Login", icon: LogIn },
    { href: "/register", label: "Register", icon: UserPlus },
    { href: "/user-details", label: "Profile", icon: User },
  ];

  return (
    <aside className="hidden lg:fixed lg:flex lg:flex-col left-0 top-0 h-full w-64 bg-gray-900 text-white pt-20 shadow-lg">
      <nav className="flex flex-col p-4 space-y-2">
        {links.map((link) => {
          const Icon = link.icon;
          const isActive = link.href === "/" ? pathname === "/" : pathname === link.href || pathname.startsWith(`${link.href}/`);
          
          return (
            <Link
              key={link.href}
              href={link.href}
              className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors ${
                isActive
                  ? "bg-blue-600 text-white"
                  : "text-gray-300 hover:bg-gray-800 hover:text-white"
              }`}
            >
              <Icon size={20} />
              <span>{link.label}</span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
