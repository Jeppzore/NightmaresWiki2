import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";
import { EntryListPage } from "../pages/EntryListPage";

describe("EntryListPage", () => {
  it("shows fetched items", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => [
          {
            type: "item",
            slug: "bronze-sword",
            title: "Bronze Sword",
            summary: "Starter weapon",
            taxonomy: ["Items", "Weapons", "Melee"],
            isDetailPage: true
          }
        ]
      }),
    );

    render(
      <MemoryRouter>
        <EntryListPage type="item" title="Items" />
      </MemoryRouter>,
    );

    await waitFor(() => expect(screen.getByText("Bronze Sword")).toBeInTheDocument());
    expect(screen.getByText("Starter weapon")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "all" })).toBeInTheDocument();
  });
});
