/* eslint-disable no-unused-vars */
import "./Dashboard.css";
import { useState, useEffect } from "react";
// import logo from "../assets/logo_export.svg";
import asc from "../assets/asc.svg";
import desc from "../assets/desc.svg";
import newNote from "../assets/new_note.svg";
import userData from "../components/usersData.json";
// import options from "../assets/dot_options.svg";
import { useNavigate } from "react-router-dom";
// import axios from "axios";
import DataCards from "../components/dataCards.jsx";

// Server information
// const hostname = "localhost";
// const port = 8080;

const Dashboard = () => {
  const navigate = useNavigate();

  // Get userID from local storage
  const userID = localStorage.getItem("userID");

  // General data
  const [usersNotes, setData] = useState([]);
  const [categories, setCategories] = useState([]);
  const [currentCategory, setCurrentCategory] = useState("All");
  const [sortOrder, setSortOrder] = useState("asc"); // Default sort order is ascending

  // NEW
  const [errorNotes, setErrorNotes] = useState("");
  const [errorCategories, setErrorCategories] = useState("");
  const [showAscDesc, setAscDesc] = useState(false);

  // const handleLogout = async() => {
  // 	localStorage.setItem("userID", -1);
  // 	localStorage.setItem("jwtToken", "");
  // 	userData.notes = [];
  // 	navigate('/');
  // };

  const handleNew = async () => {
    navigate(`/NewNote`);
  };

  const handleViewProfile = async () => {
    navigate(`/Profile`);
  };

  const filterCategory = (categoryName) => {
    setCurrentCategory(categoryName);
  };

  function sort_desc(a, b) {
    return new Date(b.last_edited) - new Date(a.last_edited);
  }

  function sort_asc(a, b) {
    return new Date(a.last_edited) - new Date(b.last_edited);
  }

  const handleSort = () => {
    const newSortOrder = sortOrder === "asc" ? "desc" : "asc";
    setSortOrder(newSortOrder);
    const sortedArray = userData.notes;

    if (newSortOrder === "asc") {
      sortedArray.sort(sort_asc);
    } else if (newSortOrder === "desc") {
      sortedArray.sort(sort_desc);
    }
    setData(sortedArray);
  };

  const toggleAscDesc = () => {
    setAscDesc(!showAscDesc);
    handleSort();
  };

  const fetchNotesData = async () => {
    setErrorNotes("");
  };

  const fetchCategoriesData = async () => {
    setErrorCategories("");
    //const idCategoryArray = [];
  };

  // useEffect(() => {
  //   setErrorNotes("");
  //   setErrorCategories("");

  //   const fetchData = async () => {};

  //   fetchData();
  // }, [userData.notes.length, categories.length]);

  return (
    <div className="dashboard-page">
      <div className="ribbon-dashboard">
        <div className="logo-dashboard">{/* <img src={logo} /> */}</div>

        <div className="search-sort-container">
          <button className="sort-img" onClick={toggleAscDesc}>
            {showAscDesc ? <img src={desc} /> : <img src={asc} />}
          </button>
        </div>
      </div>
      <div className="dashboard-container">
        <div className="side-menu"></div>
        <div className="dash-container">
          <div className="add-new-note" onClick={handleNew}>
            <img src={newNote} />
          </div>
          <div className="card-container-dash">
            <div className="card-container-grid">
              <DataCards data={usersNotes} curCategory={currentCategory} />
            </div>
            {errorNotes && (
              <div className="error-message-notes">{errorNotes}</div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
