import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, it } from "vitest";
import { Layout } from "../components/Layout";

describe("Layout", () => {
  it("renders the top masthead, navigation, and search", () => {
    render(
      <MemoryRouter initialEntries={["/enemies"]}>
        <Routes>
          <Route element={<Layout />}>
            <Route path="/enemies" element={<div>Enemies content</div>} />
          </Route>
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByRole("heading", { name: "Nightmares Wiki" })).toBeInTheDocument();
    expect(screen.getByLabelText("Header artwork")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Enemies" })).toHaveClass("site-nav__link--active");
    expect(screen.getByLabelText("Search entries")).toBeInTheDocument();
  });
});
