import React, { useState } from 'react';
import './dataCards.css';
import options from '../assets/card_options.svg';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import userData from './usersData.json';

const hostname = 'localhost';
const port = 8080;

const CardContents = ({ user, data, openSharePopup, openDeletePopup }) => {
	const navigate = useNavigate();
	const [isDropdownVisible, setIsDropdownVisible] = useState(false);

	const gotoViewNote = () => {
		// TODO: need to test auth here
		navigate(`/ViewNote`, { state: { user, data } });
	};

	const toggleDropdown = () => {
		setIsDropdownVisible(!isDropdownVisible);
	};

	const closeDropdown = () => {
		setIsDropdownVisible(false);
	};

	return (
		<div className='data-card'>
		<div className='options-container'>
			<img src={options} onClick={toggleDropdown}/>
				{isDropdownVisible && (
				<ul className="dropdown-menu" onMouseLeave={closeDropdown}>
					<li onClick={() => openSharePopup(user.id)}>share</li>
					<li onClick={() => openDeletePopup(user.id)}>delete</li>
				</ul>
				)}
		</div>
		<div className='data-card-container' onClick={gotoViewNote}>
			<p className='data-card-title'>{user.title}</p>
			<p className='data-card-lastEdited'>{user.last_edited}</p>
		</div>
		</div>
	);
};

const DataCards = ({ data, curCategory }) => {
	const [isSharePopupVisible, setSharePopupVisible] = useState(false);
	const [isDeletePopupVisible, setDeletePopupVisible] = useState(false);
	const [selectedCardId, setSelectedCardId] = useState(null);
	const [username, setUsername] = useState('');
	const [deleteError, setDeleteError] = useState('');
	const [shareError, setShareError] = useState('');
	const token = localStorage.getItem('jwtToken');
	const userID = localStorage.getItem('userID');
	const navigate = useNavigate();

	const handleShare = async() => {
		setShareError('');
		try {
			const headers = {
				'Authorization': token
			};

			console.log('Config: ', headers);

			await axios.post(`http://${hostname}:${port}/share/note/${selectedCardId}`, {
				username: username
			}, {
				headers: headers
			});
			setUsername('');
			setSharePopupVisible(false);
			setSelectedCardId(null);
		} catch (error) {
			if (error.response) {
				const message = error.response.data.message;
				if (message.includes("Access denied. No token provided.") || message.includes("Invalid or expired token.")) {
					localStorage.setItem("userID", -1);
					localStorage.setItem("jwtToken", "");
					userData.notes = [];
					navigate('/');
				}
				console.error('Could not share data: ', error.response.data.message);
				setShareError('User does not exist');
			} else {
				console.error('Could not share data: ', error.message);
				setShareError('Could not share note.');
			}
		}
	};

	const handleDelete = async() => {

		try {
			const headers = {
				'Authorization': token
			};

			console.log('Config: ', headers);

			await axios.delete(`http://${hostname}:${port}/delete/note/${selectedCardId}`, {
				headers: headers,
				data: { userID: userID }
			});

			setUsername('');
			setDeletePopupVisible(false);
			setSelectedCardId(null);
		} catch (error) {
			if (error.response) {
				const message = error.response.data.message;
				if (message.includes("Access denied. No token provided.") || message.includes("Invalid or expired token.")) {
					localStorage.setItem("userID", -1);
					localStorage.setItem("jwtToken", "");
					userData.notes = [];
					navigate('/');
				}
				console.error('Could not delete data: ', error.response.data.message);
			} else {
				console.error('Could not delete data: ', error.message);
			}
			setDeleteError('Could not delete note.');
		}
	};

	const openSharePopup = (cardId) => {
		setShareError('');
		setSelectedCardId(cardId);
		setSharePopupVisible(true);
	};

	const closeSharePopup = () => {
		setShareError('');
		handleShare();
	};

	const closeSharePopupCancel = () => {
		setUsername('');
		setSharePopupVisible(false);
		setSelectedCardId(null);
	};

	const openDeletePopup = (cardId) => {
		setDeleteError('');
		setSelectedCardId(cardId);
		setDeletePopupVisible(true);
	};

	const closeDeletePopupYes = () => {
		setDeleteError('');
		handleDelete();
	};

	const closeDeletePopupNo = () => {
		setDeletePopupVisible(false);
	};

	const filteredData = curCategory === 'All' ? data : data.filter(elem => elem.category === curCategory);

	return (
		<div className="card-container">
			{filteredData.map(elem => (
				<div key={elem.id}>
					<CardContents
						user={elem}
						data={data}
						openSharePopup={openSharePopup}
						openDeletePopup={openDeletePopup}
					/>
				</div>
			))}
			{isSharePopupVisible && selectedCardId && (
				<div className="share-popup">
					<p>Enter the username of the person you would like to share this note with.</p>
					<input className='share-popup-input'
						type="text"
						value={username}
						onChange={(e) => setUsername(e.target.value)}/>
					<div className='share-popup-buttons'>
						<button className='btn-share' onClick={closeSharePopup}>share</button>
						<button className='btn-cancel-share' onClick={closeSharePopupCancel}>cancel</button>
					</div>
					{shareError && (<div className="error-message">{shareError}</div>)}
				</div>
			)}
			{isDeletePopupVisible && selectedCardId && (
				<div className="delete-note-popup">
					<p>Are you sure you want to delete this note?</p>
					<div className='delete-popup-buttons'>
						<button className='btn-delete' onClick={closeDeletePopupYes}>yes</button>
						<button className='btn-cancel-delete' onClick={closeDeletePopupNo}>no</button>
					</div>
					{deleteError && (<div className="error-message">{deleteError}</div>)}
				</div>
			)}
		</div>
	);
};

export default DataCards;
