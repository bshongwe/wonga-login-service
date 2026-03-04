"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { Bell, Settings, LogOut, Menu, X, Home, LogIn, UserPlus, User } from "lucide-react";
import { useState } from "react";
import { useAuth } from "@/contexts/AuthContext";

export default function Navbar() {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const pathname = usePathname();
  const { isAuthenticated } = useAuth();
  const router = useRouter();

  const handleLogout = async () => {
    await fetch("/api/auth/logout", { method: "POST" });
    router.push("/login");
  };

  const links = [
    { href: "/", label: "Home", icon: Home },
    { href: "/login", label: "Login", icon: LogIn },
    { href: "/register", label: "Register", icon: UserPlus },
    { href: "/user-details", label: "Profile", icon: User },
  ];

  return (
    <header className="fixed top-0 left-0 right-0 h-16 bg-white border-b border-gray-200 shadow-sm z-20">
      <div className="flex items-center justify-between h-full px-4 sm:px-6 lg:ml-64">
        <div className="flex items-center space-x-3">
          <button
            type="button"
            aria-label="Open navigation menu"
            aria-expanded={mobileMenuOpen}
            aria-controls="mobile-menu"
            className="lg:hidden p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
          >
            {mobileMenuOpen ? <X size={20} /> : <Menu size={20} />}
          </button>
          <h1 className="text-lg sm:text-xl font-bold text-gray-800">Wonga Login Service</h1>
        </div>
        
        <div className="flex items-center space-x-2 sm:space-x-4">
          {isAuthenticated && (
            <>
              <button
                type="button"
                aria-label="Notifications"
                className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <Bell size={20} />
              </button>
              <button
                type="button"
                aria-label="Settings"
                className="hidden sm:block p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <Settings size={20} />
              </button>
              <button
                type="button"
                aria-label="Logout"
                onClick={handleLogout}
                className="flex items-center space-x-2 px-3 sm:px-4 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
              >
                <LogOut size={20} />
                <span className="hidden sm:inline">Logout</span>
              </button>
            </>
          )}
        </div>
      </div>

      {mobileMenuOpen && (
        <nav
          id="mobile-menu"
          className="lg:hidden absolute top-16 left-0 right-0 bg-white border-b border-gray-200 shadow-lg"
        >
          <div className="flex flex-col p-4 space-y-2">
            {links.map((link) => {
              const Icon = link.icon;
              const isActive = link.href === "/" ? pathname === "/" : pathname === link.href || pathname.startsWith(`${link.href}/`);
              
              return (
                <Link
                  key={link.href}
                  href={link.href}
                  onClick={() => setMobileMenuOpen(false)}
                  className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors ${
                    isActive
                      ? "bg-blue-600 text-white"
                      : "text-gray-700 hover:bg-gray-100"
                  }`}
                >
                  <Icon size={20} />
                  <span>{link.label}</span>
                </Link>
              );
            })}
          </div>
        </nav>
      )}
    </header>
  );
}
